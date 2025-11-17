// ═══════════════════════════════════════════════════════════════════════════════
// ATAS Indicator: Swing BOS/CHoCH + VP + VWAP + Big Trades
// Version: 1.0
// Platform: ATAS (Advanced Time and Sales)
// ═══════════════════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using ATAS.Indicators;
using ATAS.Indicators.Drawing;
using ATAS.Indicators.Technical;
using OFT.Attributes;
using OFT.Rendering.Context;
using OFT.Rendering.Settings;
using OFT.Rendering.Tools;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace ATAS.Indicators.Custom
{
    [DisplayName("Swing BOS/CHoCH + VWAP + Big Trades")]
    [Category("Order Flow")]
    [HelpLink("https://support.atas.net")]
    public class SwingBOS_VWAP_BigTrades : Indicator
    {
        #region ═══════ NESTED TYPES ═══════
        
        private class SwingPoint
        {
            public int Bar { get; set; }
            public decimal Price { get; set; }
            public bool IsHigh { get; set; }
            public bool IsActive { get; set; }
        }
        
        private class VolumeProfileData
        {
            public int StartBar { get; set; }
            public int EndBar { get; set; }
            public decimal HighPrice { get; set; }
            public decimal LowPrice { get; set; }
            public Dictionary<decimal, decimal> VolumeByPrice { get; set; }
            public decimal POC { get; set; }
            public decimal VAH { get; set; }
            public decimal VAL { get; set; }
            public bool IsBullish { get; set; }
        }
        
        private class BigTrade
        {
            public int Bar { get; set; }
            public decimal Price { get; set; }
            public decimal Volume { get; set; }
            public bool IsBuy { get; set; }
            public DateTime Time { get; set; }
        }
        
        #endregion
        
        #region ═══════ FIELDS ═══════
        
        private List<SwingPoint> _swingHighs = new List<SwingPoint>();
        private List<SwingPoint> _swingLows = new List<SwingPoint>();
        private List<VolumeProfileData> _volumeProfiles = new List<VolumeProfileData>();
        private List<BigTrade> _bigTrades = new List<BigTrade>();
        
        private int _currentTrend = 0; // 1 = Bull, -1 = Bear, 0 = Neutral
        private int _chochStartBar = -1;
        
        // VWAP Variables
        private int _vwapAnchor = -1;
        private bool _vwapActive = false;
        private bool _vwapBullish = true;
        private decimal _vwapSum = 0;
        private decimal _vwapVolumeSum = 0;
        private List<decimal> _vwapPrices = new List<decimal>();
        private List<decimal> _vwapVolumes = new List<decimal>();
        
        private readonly ValueDataSeries _vwapSeries = new ValueDataSeries("VWAP");
        private readonly ValueDataSeries _vwapStd1Up = new ValueDataSeries("VWAP +1σ");
        private readonly ValueDataSeries _vwapStd1Dn = new ValueDataSeries("VWAP -1σ");
        private readonly ValueDataSeries _vwapStd2Up = new ValueDataSeries("VWAP +2σ");
        private readonly ValueDataSeries _vwapStd2Dn = new ValueDataSeries("VWAP -2σ");
        
        #endregion
        
        #region ═══════ SETTINGS: MAIN ═══════
        
        [Display(GroupName = "📊 Main Settings", Name = "Swing Lookback", Order = 10)]
        [Range(1, 50)]
        public int SwingLookback { get; set; } = 2;
        
        [Display(GroupName = "📊 Main Settings", Name = "Max Swing Highs", Order = 20)]
        [Range(10, 500)]
        public int MaxSwingHighs { get; set; } = 100;
        
        [Display(GroupName = "📊 Main Settings", Name = "Max Swing Lows", Order = 30)]
        [Range(10, 500)]
        public int MaxSwingLows { get; set; } = 100;
        
        #endregion
        
        #region ═══════ SETTINGS: VWAP ═══════
        
        [Display(GroupName = "📈 VWAP", Name = "Enable VWAP System", Order = 100)]
        public bool EnableVWAP { get; set; } = true;
        
        [Display(GroupName = "📈 VWAP", Name = "Show Standard Deviation Bands", Order = 110)]
        public bool ShowVwapStdBands { get; set; } = true;
        
        [Display(GroupName = "📈 VWAP", Name = "Bullish VWAP Color", Order = 120)]
        public Color VwapBullColor { get; set; } = Color.Lime;
        
        [Display(GroupName = "📈 VWAP", Name = "Bearish VWAP Color", Order = 130)]
        public Color VwapBearColor { get; set; } = Color.Red;
        
        [Display(GroupName = "📈 VWAP", Name = "VWAP Width", Order = 140)]
        [Range(1, 5)]
        public int VwapWidth { get; set; } = 2;
        
        #endregion
        
        #region ═══════ SETTINGS: BIG TRADES ═══════
        
        [Display(GroupName = "💰 Big Trades", Name = "Enable Big Trades", Order = 200)]
        public bool EnableBigTrades { get; set; } = true;
        
        [Display(GroupName = "💰 Big Trades", Name = "Min Volume Threshold", Order = 210)]
        [Range(1, 10000)]
        public decimal BigTradeMinVolume { get; set; } = 50;
        
        [Display(GroupName = "💰 Big Trades", Name = "Big Buy Color", Order = 220)]
        public Color BigBuyColor { get; set; } = Color.FromArgb(180, 0, 255, 0);
        
        [Display(GroupName = "💰 Big Trades", Name = "Big Sell Color", Order = 230)]
        public Color BigSellColor { get; set; } = Color.FromArgb(180, 255, 0, 0);
        
        [Display(GroupName = "💰 Big Trades", Name = "Bubble Size", Order = 240)]
        [Range(5, 50)]
        public int BigTradeBubbleSize { get; set; } = 15;
        
        [Display(GroupName = "💰 Big Trades", Name = "Show Volume Label", Order = 250)]
        public bool ShowBigTradeVolume { get; set; } = true;
        
        #endregion
        
        #region ═══════ SETTINGS: SWING STYLE ═══════
        
        [Display(GroupName = "🎨 Swing High Style", Name = "Active Color", Order = 300)]
        public Color HighColor { get; set; } = Color.Red;
        
        [Display(GroupName = "🎨 Swing High Style", Name = "Broken Color", Order = 310)]
        public Color HighBrokenColor { get; set; } = Color.Gray;
        
        [Display(GroupName = "🎨 Swing Low Style", Name = "Active Color", Order = 320)]
        public Color LowColor { get; set; } = Color.Lime;
        
        [Display(GroupName = "🎨 Swing Low Style", Name = "Broken Color", Order = 330)]
        public Color LowBrokenColor { get; set; } = Color.Gray;
        
        #endregion
        
        #region ═══════ SETTINGS: BOS/CHoCH ═══════
        
        [Display(GroupName = "🔄 BOS/CHoCH", Name = "BOS Up Color", Order = 400)]
        public Color BosUpColor { get; set; } = Color.Lime;
        
        [Display(GroupName = "🔄 BOS/CHoCH", Name = "BOS Down Color", Order = 410)]
        public Color BosDownColor { get; set; } = Color.Red;
        
        [Display(GroupName = "🔄 BOS/CHoCH", Name = "CHoCH Up Color", Order = 420)]
        public Color ChochUpColor { get; set; } = Color.Cyan;
        
        [Display(GroupName = "🔄 BOS/CHoCH", Name = "CHoCH Down Color", Order = 430)]
        public Color ChochDownColor { get; set; } = Color.Orange;
        
        #endregion
        
        #region ═══════ SETTINGS: VOLUME PROFILE ═══════
        
        [Display(GroupName = "📦 Volume Profile", Name = "Enable VP", Order = 500)]
        public bool EnableVP { get; set; } = true;
        
        [Display(GroupName = "📦 Volume Profile", Name = "Show POC", Order = 510)]
        public bool ShowPOC { get; set; } = true;
        
        [Display(GroupName = "📦 Volume Profile", Name = "Show Value Area", Order = 520)]
        public bool ShowValueArea { get; set; } = true;
        
        [Display(GroupName = "📦 Volume Profile", Name = "Value Area %", Order = 530)]
        [Range(50, 90)]
        public int ValueAreaPercent { get; set; } = 70;
        
        [Display(GroupName = "📦 Volume Profile", Name = "POC Color", Order = 540)]
        public Color POCColor { get; set; } = Color.Orange;
        
        [Display(GroupName = "📦 Volume Profile", Name = "VAH/VAL Color", Order = 550)]
        public Color VAColor { get; set; } = Color.Blue;
        
        #endregion
        
        #region ═══════ CONSTRUCTOR ═══════
        
        public SwingBOS_VWAP_BigTrades()
            : base(true)
        {
            DenyToChangePanel = true;
            EnableCustomDrawing = true;
            SubscribeToDrawingEvents(DrawingLayouts.Historical | DrawingLayouts.Final);
            
            DataSeries[0] = _vwapSeries;
            DataSeries.Add(_vwapStd1Up);
            DataSeries.Add(_vwapStd1Dn);
            DataSeries.Add(_vwapStd2Up);
            DataSeries.Add(_vwapStd2Dn);
        }
        
        #endregion
        
        #region ═══════ OVERRIDE: OnCalculate ═══════
        
        protected override void OnCalculate(int bar, decimal value)
        {
            if (bar < SwingLookback * 2)
                return;
            
            // ═══ SWING DETECTION ═══
            DetectSwingHighs(bar);
            DetectSwingLows(bar);
            
            // ═══ BOS/CHoCH DETECTION ═══
            CheckSwingBreaks(bar);
            
            // ═══ VWAP CALCULATION ═══
            if (EnableVWAP && _vwapActive)
            {
                CalculateVWAP(bar);
            }
            
            // ═══ BIG TRADES DETECTION ═══
            if (EnableBigTrades)
            {
                DetectBigTrades(bar);
            }
            
            // ═══ VOLUME PROFILE UPDATE ═══
            if (EnableVP && _chochStartBar >= 0)
            {
                UpdateCurrentVolumeProfile(bar);
            }
        }
        
        #endregion
        
        #region ═══════ SWING DETECTION ═══════
        
        private void DetectSwingHighs(int bar)
        {
            if (bar < SwingLookback * 2)
                return;
            
            int pivotBar = bar - SwingLookback;
            decimal pivotHigh = GetCandle(pivotBar).High;
            
            bool isSwingHigh = true;
            
            // Check left side
            for (int i = 1; i <= SwingLookback; i++)
            {
                if (GetCandle(pivotBar - i).High >= pivotHigh)
                {
                    isSwingHigh = false;
                    break;
                }
            }
            
            // Check right side
            if (isSwingHigh)
            {
                for (int i = 1; i <= SwingLookback; i++)
                {
                    if (GetCandle(pivotBar + i).High > pivotHigh)
                    {
                        isSwingHigh = false;
                        break;
                    }
                }
            }
            
            if (isSwingHigh)
            {
                var swing = new SwingPoint
                {
                    Bar = pivotBar,
                    Price = pivotHigh,
                    IsHigh = true,
                    IsActive = true
                };
                
                _swingHighs.Add(swing);
                
                // Limit array size
                if (_swingHighs.Count > MaxSwingHighs)
                    _swingHighs.RemoveAt(0);
            }
        }
        
        private void DetectSwingLows(int bar)
        {
            if (bar < SwingLookback * 2)
                return;
            
            int pivotBar = bar - SwingLookback;
            decimal pivotLow = GetCandle(pivotBar).Low;
            
            bool isSwingLow = true;
            
            // Check left side
            for (int i = 1; i <= SwingLookback; i++)
            {
                if (GetCandle(pivotBar - i).Low <= pivotLow)
                {
                    isSwingLow = false;
                    break;
                }
            }
            
            // Check right side
            if (isSwingLow)
            {
                for (int i = 1; i <= SwingLookback; i++)
                {
                    if (GetCandle(pivotBar + i).Low < pivotLow)
                    {
                        isSwingLow = false;
                        break;
                    }
                }
            }
            
            if (isSwingLow)
            {
                var swing = new SwingPoint
                {
                    Bar = pivotBar,
                    Price = pivotLow,
                    IsHigh = false,
                    IsActive = true
                };
                
                _swingLows.Add(swing);
                
                // Limit array size
                if (_swingLows.Count > MaxSwingLows)
                    _swingLows.RemoveAt(0);
            }
        }
        
        #endregion
        
        #region ═══════ BOS/CHoCH DETECTION ═══════
        
        private void CheckSwingBreaks(int bar)
        {
            var candle = GetCandle(bar);
            
            // Check Swing High Breaks
            foreach (var swing in _swingHighs.Where(s => s.IsActive).ToList())
            {
                if (candle.Close > swing.Price && candle.High > swing.Price)
                {
                    swing.IsActive = false;
                    
                    bool isChoch = _currentTrend == -1;
                    
                    if (isChoch)
                    {
                        // CHoCH detected - Start new VWAP
                        if (EnableVWAP)
                        {
                            StartVWAP(bar, true);
                        }
                        
                        // Create Volume Profile for previous trend
                        if (EnableVP && _chochStartBar >= 0)
                        {
                            CreateVolumeProfile(_chochStartBar, bar);
                        }
                        
                        _chochStartBar = bar;
                        _currentTrend = 1;
                    }
                    else
                    {
                        // BOS detected
                        _currentTrend = 1;
                    }
                }
            }
            
            // Check Swing Low Breaks
            foreach (var swing in _swingLows.Where(s => s.IsActive).ToList())
            {
                if (candle.Close < swing.Price && candle.Low < swing.Price)
                {
                    swing.IsActive = false;
                    
                    bool isChoch = _currentTrend == 1;
                    
                    if (isChoch)
                    {
                        // CHoCH detected - Start new VWAP
                        if (EnableVWAP)
                        {
                            StartVWAP(bar, false);
                        }
                        
                        // Create Volume Profile for previous trend
                        if (EnableVP && _chochStartBar >= 0)
                        {
                            CreateVolumeProfile(_chochStartBar, bar);
                        }
                        
                        _chochStartBar = bar;
                        _currentTrend = -1;
                    }
                    else
                    {
                        // BOS detected
                        _currentTrend = -1;
                    }
                }
            }
        }
        
        #endregion
        
        #region ═══════ VWAP CALCULATION ═══════
        
        private void StartVWAP(int bar, bool isBullish)
        {
            _vwapAnchor = bar;
            _vwapActive = true;
            _vwapBullish = isBullish;
            _vwapSum = 0;
            _vwapVolumeSum = 0;
            _vwapPrices.Clear();
            _vwapVolumes.Clear();
        }
        
        private void CalculateVWAP(int bar)
        {
            if (_vwapAnchor < 0 || bar < _vwapAnchor)
                return;
            
            var candle = GetCandle(bar);
            decimal typicalPrice = (candle.High + candle.Low + candle.Close) / 3;
            decimal vol = candle.Volume;
            
            _vwapPrices.Add(typicalPrice);
            _vwapVolumes.Add(vol);
            
            _vwapSum += typicalPrice * vol;
            _vwapVolumeSum += vol;
            
            if (_vwapVolumeSum > 0)
            {
                decimal vwap = _vwapSum / _vwapVolumeSum;
                _vwapSeries[bar] = vwap;
                
                // Calculate Standard Deviation
                if (ShowVwapStdBands && _vwapPrices.Count > 1)
                {
                    decimal sumSquaredDiff = 0;
                    
                    for (int i = 0; i < _vwapPrices.Count; i++)
                    {
                        decimal diff = _vwapPrices[i] - vwap;
                        sumSquaredDiff += (diff * diff) * _vwapVolumes[i];
                    }
                    
                    decimal variance = sumSquaredDiff / _vwapVolumeSum;
                    decimal stdDev = (decimal)Math.Sqrt((double)variance);
                    
                    _vwapStd1Up[bar] = vwap + stdDev;
                    _vwapStd1Dn[bar] = vwap - stdDev;
                    _vwapStd2Up[bar] = vwap + stdDev * 2;
                    _vwapStd2Dn[bar] = vwap - stdDev * 2;
                }
            }
        }
        
        #endregion
        
        #region ═══════ BIG TRADES DETECTION ═══════
        
        private void DetectBigTrades(int bar)
        {
            var candle = GetCandle(bar);
            
            // In ATAS you have access to CumulativeTrades or Tick data
            // This is a simplified version - in real ATAS you'd use:
            // var trades = candle.GetValue(CumulativeTrades);
            
            // For demonstration, we detect based on volume spikes
            if (candle.Volume >= BigTradeMinVolume)
            {
                bool isBuy = candle.Close > candle.Open;
                
                var bigTrade = new BigTrade
                {
                    Bar = bar,
                    Price = isBuy ? candle.High : candle.Low,
                    Volume = candle.Volume,
                    IsBuy = isBuy,
                    Time = candle.Time
                };
                
                _bigTrades.Add(bigTrade);
                
                // Keep only recent trades
                if (_bigTrades.Count > 1000)
                    _bigTrades.RemoveAt(0);
            }
        }
        
        #endregion
        
        #region ═══════ VOLUME PROFILE ═══════
        
        private void CreateVolumeProfile(int startBar, int endBar)
        {
            var vp = new VolumeProfileData
            {
                StartBar = startBar,
                EndBar = endBar,
                VolumeByPrice = new Dictionary<decimal, decimal>()
            };
            
            decimal high = decimal.MinValue;
            decimal low = decimal.MaxValue;
            
            // Calculate range and accumulate volume
            for (int i = startBar; i <= endBar; i++)
            {
                var candle = GetCandle(i);
                high = Math.Max(high, candle.High);
                low = Math.Min(low, candle.Low);
                
                // Simplified: assign volume to close price
                // In real implementation, distribute across price levels
                decimal priceLevel = Math.Round(candle.Close, 2);
                
                if (vp.VolumeByPrice.ContainsKey(priceLevel))
                    vp.VolumeByPrice[priceLevel] += candle.Volume;
                else
                    vp.VolumeByPrice[priceLevel] = candle.Volume;
            }
            
            vp.HighPrice = high;
            vp.LowPrice = low;
            
            // Calculate POC (Point of Control)
            if (vp.VolumeByPrice.Any())
            {
                var maxVolPair = vp.VolumeByPrice.OrderByDescending(x => x.Value).First();
                vp.POC = maxVolPair.Key;
                
                // Calculate Value Area
                CalculateValueArea(vp);
            }
            
            _volumeProfiles.Add(vp);
            
            // Keep limited number
            if (_volumeProfiles.Count > 50)
                _volumeProfiles.RemoveAt(0);
        }
        
        private void CalculateValueArea(VolumeProfileData vp)
        {
            decimal totalVolume = vp.VolumeByPrice.Values.Sum();
            decimal targetVolume = totalVolume * ValueAreaPercent / 100m;
            
            var sortedByPrice = vp.VolumeByPrice.OrderBy(x => x.Key).ToList();
            int pocIndex = sortedByPrice.FindIndex(x => x.Key == vp.POC);
            
            decimal vaVolume = sortedByPrice[pocIndex].Value;
            int upperIdx = pocIndex;
            int lowerIdx = pocIndex;
            
            while (vaVolume < targetVolume && (upperIdx < sortedByPrice.Count - 1 || lowerIdx > 0))
            {
                decimal upperVol = upperIdx < sortedByPrice.Count - 1 ? sortedByPrice[upperIdx + 1].Value : 0;
                decimal lowerVol = lowerIdx > 0 ? sortedByPrice[lowerIdx - 1].Value : 0;
                
                if (upperVol > lowerVol && upperIdx < sortedByPrice.Count - 1)
                {
                    upperIdx++;
                    vaVolume += upperVol;
                }
                else if (lowerIdx > 0)
                {
                    lowerIdx--;
                    vaVolume += lowerVol;
                }
                else
                    break;
            }
            
            vp.VAH = sortedByPrice[upperIdx].Key;
            vp.VAL = sortedByPrice[lowerIdx].Key;
        }
        
        private void UpdateCurrentVolumeProfile(int bar)
        {
            // Update live volume profile in OnRender
        }
        
        #endregion
        
        #region ═══════ CUSTOM DRAWING ═══════
        
        protected override void OnRender(RenderContext context, DrawingLayouts layout)
        {
            if (ChartInfo == null || Container == null)
                return;
            
            // ═══ DRAW SWING LINES ═══
            DrawSwingLines(context);
            
            // ═══ DRAW BIG TRADES (BUBBLES) ═══
            if (EnableBigTrades)
                DrawBigTrades(context);
            
            // ═══ DRAW VOLUME PROFILES ═══
            if (EnableVP)
                DrawVolumeProfiles(context);
        }
        
        private void DrawSwingLines(RenderContext context)
        {
            // Draw Swing Highs
            foreach (var swing in _swingHighs)
            {
                var color = swing.IsActive ? HighColor : HighBrokenColor;
                var pen = new Pen(color, 2);
                
                int x = ChartInfo.GetXByBar(swing.Bar);
                int y = ChartInfo.GetYByPrice(swing.Price);
                int xEnd = ChartInfo.GetXByBar(CurrentBar - 1);
                
                context.DrawLine(pen, x, y, xEnd, y);
            }
            
            // Draw Swing Lows
            foreach (var swing in _swingLows)
            {
                var color = swing.IsActive ? LowColor : LowBrokenColor;
                var pen = new Pen(color, 2);
                
                int x = ChartInfo.GetXByBar(swing.Bar);
                int y = ChartInfo.GetYByPrice(swing.Price);
                int xEnd = ChartInfo.GetXByBar(CurrentBar - 1);
                
                context.DrawLine(pen, x, y, xEnd, y);
            }
        }
        
        private void DrawBigTrades(RenderContext context)
        {
            foreach (var trade in _bigTrades)
            {
                if (trade.Bar < 0 || trade.Bar >= CurrentBar)
                    continue;
                
                int x = ChartInfo.GetXByBar(trade.Bar);
                int y = ChartInfo.GetYByPrice(trade.Price);
                
                // Calculate bubble size based on volume
                float size = Math.Min(BigTradeBubbleSize + (float)(trade.Volume / BigTradeMinVolume) * 5, 50);
                
                var color = trade.IsBuy ? BigBuyColor : BigSellColor;
                var brush = new SolidBrush(color);
                
                // Draw bubble (circle)
                context.Graphics.FillEllipse(brush, x - size / 2, y - size / 2, size, size);
                
                // Draw border
                var borderPen = new Pen(Color.FromArgb(255, color), 2);
                context.Graphics.DrawEllipse(borderPen, x - size / 2, y - size / 2, size, size);
                
                // Draw volume label
                if (ShowBigTradeVolume)
                {
                    string volumeText = trade.Volume.ToString("F0");
                    var font = new Font("Arial", 8, FontStyle.Bold);
                    var textBrush = new SolidBrush(Color.White);
                    var textSize = context.Graphics.MeasureString(volumeText, font);
                    
                    context.Graphics.DrawString(
                        volumeText,
                        font,
                        textBrush,
                        x - textSize.Width / 2,
                        y - textSize.Height / 2
                    );
                }
                
                brush.Dispose();
                borderPen.Dispose();
            }
        }
        
        private void DrawVolumeProfiles(RenderContext context)
        {
            foreach (var vp in _volumeProfiles)
            {
                if (vp.StartBar < 0 || vp.EndBar >= CurrentBar)
                    continue;
                
                int xStart = ChartInfo.GetXByBar(vp.StartBar);
                int xEnd = ChartInfo.GetXByBar(vp.EndBar);
                
                // Draw POC
                if (ShowPOC)
                {
                    int yPoc = ChartInfo.GetYByPrice(vp.POC);
                    var pocPen = new Pen(POCColor, 3);
                    context.DrawLine(pocPen, xStart, yPoc, xEnd, yPoc);
                }
                
                // Draw Value Area
                if (ShowValueArea)
                {
                    int yVah = ChartInfo.GetYByPrice(vp.VAH);
                    int yVal = ChartInfo.GetYByPrice(vp.VAL);
                    var vaPen = new Pen(VAColor, 2);
                    vaPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    
                    context.DrawLine(vaPen, xStart, yVah, xEnd, yVah);
                    context.DrawLine(vaPen, xStart, yVal, xEnd, yVal);
                }
            }
        }
        
        #endregion
    }
}
