# All-Swing-BoS-ChoCh//@version=6
indicator("Swing BOS/CHoCH + VP + VWAP Complete", overlay=true, max_lines_count=500, max_labels_count=500, max_boxes_count=500, max_bars_back=5000)

//═════════════════════════════════════════════════════════════════════════════════
// 📊 MAIN SETTINGS
//═════════════════════════════════════════════════════════════════════════════════

lookback        = input.int(2,          'Swing Lookback',               group='📊 Main Settings', minval=1, maxval=50)
maxSwingsHigh   = input.int(500,        'Max Swing Highs',              group='📊 Main Settings', minval=10, maxval=5000, step=50)
maxSwingsLow    = input.int(500,        'Max Swing Lows',               group='📊 Main Settings', minval=10, maxval=5000, step=50)

//═════════════════════════════════════════════════════════════════════════════════
// 📈 VWAP SETTINGS (NEU!)
//═════════════════════════════════════════════════════════════════════════════════

enableVWAP = input(true, 'Enable VWAP System', group='📈 VWAP')

vwapBullColor = input.color(#00ff00, "Bullish VWAP", group="📈 VWAP Style", inline="vw1")
vwapBearColor = input.color(#ff0000, "Bearish VWAP", group="📈 VWAP Style", inline="vw1")
vwapWidth = input.int(2, "Width", minval=1, maxval=5, group="📈 VWAP Style", inline="vw1")

showVwapStd1 = input(true, "±1 StdDev", group="📈 VWAP Bands", inline="vs1")
vwapStd1Color = input.color(color.new(#2196F3, 30), "", group="📈 VWAP Bands", inline="vs1")

showVwapStd2 = input(true, "±2 StdDev", group="📈 VWAP Bands", inline="vs2")
vwapStd2Color = input.color(color.new(#9C27B0, 40), "", group="📈 VWAP Bands", inline="vs2")

showVwapStd3 = input(false, "±3 StdDev", group="📈 VWAP Bands", inline="vs3")
vwapStd3Color = input.color(color.new(#FF5722, 50), "", group="📈 VWAP Bands", inline="vs3")

showVwapFill = input(true, "Show Fills", group="📈 VWAP Bands")
vwapFillAlpha = input.int(94, "Fill Transparency", minval=80, maxval=98, group="📈 VWAP Bands")

//═════════════════════════════════════════════════════════════════════════════════
// 🎨 SWING HIGH STYLE
//═════════════════════════════════════════════════════════════════════════════════

highColor       = input.color(color.red,        'Active Color',         group='🎨 Swing High Style', inline='high1')
highWidth       = input.int(2,                  'Width',                group='🎨 Swing High Style', inline='high1', minval=1, maxval=10)
highStyleStr    = input.string('solid',         'Style',                group='🎨 Swing High Style', inline='high1', options=['solid', 'dashed', 'dotted'])

highFrozenColor = input.color(color.gray,       'Broken Color',         group='🎨 Swing High Style', inline='high2')
highFrozenWidth = input.int(1,                  'Width',                group='🎨 Swing High Style', inline='high2', minval=1, maxval=10)
highFrozenStyleStr = input.string('solid',      'Style',                group='🎨 Swing High Style', inline='high2', options=['solid', 'dashed', 'dotted'])

//═════════════════════════════════════════════════════════════════════════════════
// 🎨 SWING LOW STYLE
//═════════════════════════════════════════════════════════════════════════════════

lowColor        = input.color(color.green,      'Active Color',         group='🎨 Swing Low Style', inline='low1')
lowWidth        = input.int(2,                  'Width',                group='🎨 Swing Low Style', inline='low1', minval=1, maxval=10)
lowStyleStr     = input.string('solid',         'Style',                group='🎨 Swing Low Style', inline='low1', options=['solid', 'dashed', 'dotted'])

lowFrozenColor  = input.color(color.gray,       'Broken Color',         group='🎨 Swing Low Style', inline='low2')
lowFrozenWidth  = input.int(1,                  'Width',                group='🎨 Swing Low Style', inline='low2', minval=1, maxval=10)
lowFrozenStyleStr = input.string('solid',       'Style',                group='🎨 Swing Low Style', inline='low2', options=['solid', 'dashed', 'dotted'])

//═════════════════════════════════════════════════════════════════════════════════
// 🔄 BOS SETTINGS - SEPARATE COLORS
//═════════════════════════════════════════════════════════════════════════════════

bosUpColor      = input.color(#00ff00,          'BOS ↑ Color (Bullish Trend)',  group='🔄 BOS Settings', inline='bos1')
bosDownColor    = input.color(#ff0000,          'BOS ↓ Color (Bearish Trend)',  group='🔄 BOS Settings', inline='bos2')

bosLabelSize    = input.string('normal',        'Label Size',           group='🔄 BOS Settings', inline='bos3', options=['tiny', 'small', 'normal', 'large'])
bosLabelPosition = input.string('On Line',      'Label Position',       group='🔄 BOS Settings', inline='bos4',
     options=['On Line', 'Center of Line'])

showBosLabels   = input(true,               'Show BOS Info Labels',     group='🔄 BOS Info Labels')
bosInfoPosition = input.string('Top Right', 'Label Position',           group='🔄 BOS Info Labels',
     options=['Top Left', 'Top Right', 'Bottom Left', 'Bottom Right'])
bosInfoSize     = input.string('tiny',      'Label Size',               group='🔄 BOS Info Labels',
     options=['tiny', 'small', 'normal'])

showBosInfoBars = input(true,               'Show Bar Count',           group='🔄 BOS Info Content')
showBosInfoRange = input(false,             'Show Price Range',         group='🔄 BOS Info Content')

//═════════════════════════════════════════════════════════════════════════════════
// 🔀 CHOCH SETTINGS
//═════════════════════════════════════════════════════════════════════════════════

chochUpColor    = input.color(#00ff00,          'CHoCH ↑ Color',        group='🔀 CHoCH Settings', inline='choch1')
chochDownColor  = input.color(#ff0000,          'CHoCH ↓ Color',        group='🔀 CHoCH Settings', inline='choch2')
chochLabelSize  = input.string('large',         'Label Size',           group='🔀 CHoCH Settings', inline='choch3',
     options=['tiny', 'small', 'normal', 'large'])
chochLabelPosition = input.string('On Line',    'Label Position',       group='🔀 CHoCH Settings', inline='choch4',
     options=['On Line', 'Center of Line'])

//═════════════════════════════════════════════════════════════════════════════════
// 📦 VOLUME PROFILE - MAIN
//═════════════════════════════════════════════════════════════════════════════════

enableVP        = input(true,           'Enable Volume Profile System', group='📦 VP Main')
maxVPBoxes      = input.int(50,         'Max VP Boxes',                 group='📦 VP Main', minval=5, maxval=100)

//═════════════════════════════════════════════════════════════════════════════════
// 📦 VP COMPONENTS
//═════════════════════════════════════════════════════════════════════════════════

showVpRangeBox  = input(true,           'Show Range Boxes',             group='📦 VP Components')
showVpHistogram = input(false,          'Show Histograms',              group='📦 VP Components')
showCurrentVP   = input(true,           'Show CURRENT VP',              group='📦 VP Components')
showFrozenVP    = input(true,           'Show FROZEN VPs',              group='📦 VP Components')

//═════════════════════════════════════════════════════════════════════════════════
// 📦 VP LEVELS
//═════════════════════════════════════════════════════════════════════════════════

showPOC         = input(true,           'Show POC',                     group='📦 VP Levels')
showVAH         = input(true,           'Show VAH',                     group='📦 VP Levels')
showVAL         = input(true,           'Show VAL',                     group='📦 VP Levels')
valueAreaPct    = input.float(70,       'Value Area %',                 group='📦 VP Levels', minval=50, maxval=90, step=5)

useDevelopingVA = input(true,           'Use DEVELOPING Value Area',    group='📦 VP Levels',
     tooltip='VAH/VAL ändern sich LIVE mit jeder Kerze')
showDevelopingVABox = input(true,       'Show Developing VA Box',       group='📦 VP Levels')
developingVAColor = input.color(#9C27B0, 'Developing VA Color',         group='📦 VP Levels')
developingVATransp = input.int(90,      'Developing VA Transparency',   group='📦 VP Levels', minval=70, maxval=98)

//═════════════════════════════════════════════════════════════════════════════════
// 🎨 POC STYLE
//═════════════════════════════════════════════════════════════════════════════════

pocColor        = input.color(#FF6D00,          'POC Color',            group='🎨 POC Style', inline='poc')
pocWidth        = input.int(3,                  'Width',                group='🎨 POC Style', inline='poc', minval=1, maxval=10)
pocStyleStr     = input.string('solid',         'Style',                group='🎨 POC Style', inline='poc', options=['solid', 'dashed', 'dotted'])

//═════════════════════════════════════════════════════════════════════════════════
// 🎨 VALUE AREA STYLE
//═════════════════════════════════════════════════════════════════════════════════

vahColor        = input.color(#2196F3,          'VAH Color',            group='🎨 Value Area Style', inline='vah')
vahWidth        = input.int(2,                  'Width',                group='🎨 Value Area Style', inline='vah', minval=1, maxval=10)
vahStyleStr     = input.string('dashed',        'Style',                group='🎨 Value Area Style', inline='vah', options=['solid', 'dashed', 'dotted'])

valColor        = input.color(#FF6D00,          'VAL Color',            group='🎨 Value Area Style', inline='val')
valWidth        = input.int(2,                  'Width',                group='🎨 Value Area Style', inline='val', minval=1, maxval=10)
valStyleStr     = input.string('dashed',        'Style',                group='🎨 Value Area Style', inline='val', options=['solid', 'dashed', 'dotted'])

//═════════════════════════════════════════════════════════════════════════════════
// 🎨 CURRENT VP STYLE
//═════════════════════════════════════════════════════════════════════════════════

vpCurrentColor  = input.color(color.yellow,     'Current VP Color',     group='🎨 Current VP Style', inline='cur')
vpCurrentWidth  = input.int(3,                  'Width',                group='🎨 Current VP Style', inline='cur', minval=1, maxval=10)
vpCurrentStyleStr = input.string('solid',       'Style',                group='🎨 Current VP Style', inline='cur', options=['solid', 'dashed', 'dotted'])

//═════════════════════════════════════════════════════════════════════════════════
// 🎨 VP BOX COLORS
//═════════════════════════════════════════════════════════════════════════════════

vpBullBoxColor  = input.color(color.blue,       'Bullish Box (Close>Open)', group='🎨 VP Box Colors', inline='box1')
vpBearBoxColor  = input.color(color.red,        'Bearish Box (Close<Open)', group='🎨 VP Box Colors', inline='box2')

vpBoxBgTransparency = input.int(95,     'Box Transparency',             group='🎨 VP Box Colors', minval=85, maxval=98)
vpBoxBorderWidth = input.int(2,         'Border Width',                 group='🎨 VP Box Colors', minval=1, maxval=5)

vpBullHistColor = input.color(#26A69A,          'Bullish Histogram',    group='🎨 VP Box Colors', inline='hist1')
vpBearHistColor = input.color(#EF5350,          'Bearish Histogram',    group='🎨 VP Box Colors', inline='hist2')

//═════════════════════════════════════════════════════════════════════════════════
// 📊 VP HISTOGRAM SETTINGS
//═════════════════════════════════════════════════════════════════════════════════

vpRows          = input.int(24,         'Histogram Bars',               group='📊 VP Histogram', minval=10, maxval=50)
vpHistStyle     = input.string('Filled Bars', 'Style',                  group='📊 VP Histogram',
     options=['Filled Bars', 'Thin Lines'])
vpBarSpacing    = input.float(0.8,      'Bar Spacing',                  group='📊 VP Histogram', minval=0.5, maxval=1.0, step=0.1)

vpHistAlignment = input.string('Extended Over Range', 'Histogram Alignment',  group='📊 VP Histogram',
     options=['Right Aligned', 'Extended Over Range'])
vpMaxWidth      = input.float(60,       'Max Width % (Right Aligned)',  group='📊 VP Histogram', minval=20, maxval=100, step=5)
vpHistTransparency = input.int(60,      'Transparency',                 group='📊 VP Histogram', minval=0, maxval=95)

//═════════════════════════════════════════════════════════════════════════════════
// 🏷️ VP BOX LABELS
//═════════════════════════════════════════════════════════════════════════════════

showVpBoxLabels = input(true,           'Show VP Box Labels',           group='🏷️ VP Box Labels')
vpBoxLabelPosition = input.string('Top Right', 'Position',              group='🏷️ VP Box Labels',
     options=['Top Left', 'Top Right', 'Bottom Left', 'Bottom Right'])
vpBoxLabelSize  = input.string('small', 'Size',                         group='🏷️ VP Box Labels',
     options=['tiny', 'small', 'normal'])

showLabelTitle  = input(true,           'Show Title',                   group='🏷️ VP Label Content')
showLabelBars   = input(true,           'Show Bar Count',               group='🏷️ VP Label Content')
showLabelRange  = input(true,           'Show Price Range',             group='🏷️ VP Label Content')
showLabelPOC    = input(true,           'Show POC',                     group='🏷️ VP Label Content')
showLabelVAH    = input(true,           'Show VAH',                     group='🏷️ VP Label Content')
showLabelVAL    = input(true,           'Show VAL',                     group='🏷️ VP Label Content')

//═════════════════════════════════════════════════════════════════════════════════
// 🟣 ANCHORED VALUE AREA
//═════════════════════════════════════════════════════════════════════════════════

showAnchoredVA  = input(true,           'Show Anchored VA',             group='🟣 Anchored VA')
anchoredVAColor = input.color(color.purple, 'Color',                    group='🟣 Anchored VA', inline='ava')
anchoredVAWidth = input.int(2,          'Width',                        group='🟣 Anchored VA', inline='ava', minval=1, maxval=10)
anchoredVAStyle = input.string('solid', 'Style',                        group='🟣 Anchored VA', inline='ava', options=['solid', 'dashed', 'dotted'])
anchoredVATransp = input.int(0,         'Transparency',                 group='🟣 Anchored VA', minval=0, maxval=100)

//═════════════════════════════════════════════════════════════════════════════════
// HELPER FUNCTIONS
//═════════════════════════════════════════════════════════════════════════════════

getLineStyle(string s) =>
    s == 'solid' ? line.style_solid : s == 'dashed' ? line.style_dashed : line.style_dotted

getLabelSize(string s) =>
    s == 'tiny' ? size.tiny : s == 'small' ? size.small : s == 'large' ? size.large : size.normal

highStyle = getLineStyle(highStyleStr)
highFrozenStyle = getLineStyle(highFrozenStyleStr)
lowStyle = getLineStyle(lowStyleStr)
lowFrozenStyle = getLineStyle(lowFrozenStyleStr)
pocStyle = getLineStyle(pocStyleStr)
vahStyle = getLineStyle(vahStyleStr)
valStyle = getLineStyle(valStyleStr)
vpCurrentStyle = getLineStyle(vpCurrentStyleStr)
anchoredVALineStyle = getLineStyle(anchoredVAStyle)

//═════════════════════════════════════════════════════════════════════════════════
// TYPES
//═════════════════════════════════════════════════════════════════════════════════

type VolumeProfileData
    int startBar
    int endBar
    int startTime
    int endTime
    float highPrice
    float lowPrice
    float openPrice
    float closePrice
    array<float> volumes
    float poc
    float vah
    float val
    box rangeBox
    line pocLine
    line vahLine
    line valLine
    box developingVABox
    label infoLabel
    array<box> histogramBoxes
    bool isBullish

//═════════════════════════════════════════════════════════════════════════════════
// GLOBAL VARS
//═════════════════════════════════════════════════════════════════════════════════

var array<line> swingHighLines      = array.new<line>()
var array<float> swingHighVals      = array.new<float>()
var array<int> swingHighBars        = array.new<int>()
var array<bool> swingHighActive     = array.new<bool>()

var array<line> swingLowLines       = array.new<line>()
var array<float> swingLowVals       = array.new<float>()
var array<int> swingLowBars         = array.new<int>()
var array<bool> swingLowActive      = array.new<bool>()

var int currentTrend = 0
var int chochStartBar = na
var int chochStartTime = na

var array<VolumeProfileData> volumeProfiles = array.new<VolumeProfileData>()

var box currentVpBox = na
var line currentPocLine = na
var line currentVahLine = na
var line currentValLine = na
var box currentDevelopingVABox = na
var label currentVpLabel = na

var line anchoredVAH = na
var line anchoredVAL = na
var label anchoredVALabel = na
var array<float> cumulativePrices = array.new<float>()
var array<float> cumulativeVolumes = array.new<float>()

// VWAP State (NEU!)
var int vwapAnchor = na
var bool vwapActive = false
var bool vwapBullish = true

//═════════════════════════════════════════════════════════════════════════════════
// 🧮 VWAP CALCULATION (NEU!)
//═════════════════════════════════════════════════════════════════════════════════

calculateVWAP() =>
    if na(vwapAnchor) or not vwapActive
        [na, na]
    else
        float sumPV = 0.0
        float sumV = 0.0
        float sumSquaredDiff = 0.0
        
        int bars = bar_index - vwapAnchor
        
        // First pass: Calculate VWAP
        for i = 0 to bars
            float tp = hlc3[i]
            float vol = volume[i]
            sumPV += tp * vol
            sumV += vol
        
        float vwap = sumV > 0 ? sumPV / sumV : na
        
        // Second pass: Calculate StdDev (CORRECT!)
        if not na(vwap)
            for i = 0 to bars
                float tp = hlc3[i]
                float vol = volume[i]
                float diff = tp - vwap
                sumSquaredDiff += (diff * diff) * vol
        
        float variance = sumV > 0 ? sumSquaredDiff / sumV : 0
        float stdDev = variance > 0 ? math.sqrt(variance) : 0
        
        [vwap, stdDev]

//═════════════════════════════════════════════════════════════════════════════════
// SWING DETECTION
//═════════════════════════════════════════════════════════════════════════════════

enoughBars = bar_index >= lookback * 2
float highestInWindow = ta.highest(high, lookback * 2 + 1)
float lowestInWindow = ta.lowest(low, lookback * 2 + 1)

isSwingHigh = enoughBars and highestInWindow == high[lookback]
isSwingLow  = enoughBars and lowestInWindow == low[lookback]

//═════════════════════════════════════════════════════════════════════════════════
// VOLUME PROFILE CALCULATION (DEIN ORIGINAL!)
//═════════════════════════════════════════════════════════════════════════════════

calculateVolumeProfile(int startBar, int endBar, int startTime, int endTime) =>
    float rangeHigh = high[bar_index - startBar]
    float rangeLow = low[bar_index - startBar]
    float rangeOpen = open[bar_index - startBar]
    float rangeClose = close
    
    for i = startBar to endBar
        if bar_index - i >= 0
            rangeHigh := math.max(rangeHigh, high[bar_index - i])
            rangeLow := math.min(rangeLow, low[bar_index - i])
    
    float priceRange = rangeHigh - rangeLow
    float rowHeight = priceRange / vpRows
    
    array<float> volumes = array.new<float>(vpRows, 0.0)
    array<float> prices = array.new<float>(vpRows, 0.0)
    
    if priceRange > 0
        for i = startBar to endBar
            if bar_index - i >= 0
                float barHigh = high[bar_index - i]
                float barLow = low[bar_index - i]
                float barVolume = volume[bar_index - i]
                
                for row = 0 to vpRows - 1
                    float rowPrice = rangeLow + (row + 0.5) * rowHeight
                    
                    if barLow <= rowPrice and rowPrice <= barHigh
                        prices.set(row, rowPrice)
                        float currentVol = volumes.get(row)
                        volumes.set(row, currentVol + barVolume)
    
    float maxVolume = volumes.max()
    float poc = na
    float vah = na
    float val = na
    
    if maxVolume > 0
        int pocIndex = volumes.indexof(maxVolume)
        poc := prices.get(pocIndex)
        
        float totalVolume = volumes.sum()
        if totalVolume > 0
            float targetVolume = totalVolume * (valueAreaPct / 100)
            float vaVolume = volumes.get(pocIndex)
            int upperIndex = pocIndex
            int lowerIndex = pocIndex
            
            while vaVolume < targetVolume and (upperIndex < vpRows - 1 or lowerIndex > 0)
                float upperVol = upperIndex < vpRows - 1 ? volumes.get(upperIndex + 1) : 0
                float lowerVol = lowerIndex > 0 ? volumes.get(lowerIndex - 1) : 0
                
                if upperVol > lowerVol and upperIndex < vpRows - 1
                    upperIndex += 1
                    vaVolume += upperVol
                else if lowerIndex > 0
                    lowerIndex -= 1
                    vaVolume += lowerVol
                else
                    break
            
            vah := prices.get(upperIndex)
            val := prices.get(lowerIndex)
    
    bool isBullish = rangeClose > rangeOpen
    
    VolumeProfileData.new(
         startBar, endBar, startTime, endTime,
         rangeHigh, rangeLow, rangeOpen, rangeClose,
         volumes, poc, vah, val,
         na, na, na, na, na, na,
         array.new<box>(),
         isBullish)

drawVolumeProfile(VolumeProfileData vp, bool isFrozen = true) =>
    if not na(vp.poc)
        float maxVolume = vp.volumes.max()
        
        if maxVolume > 0
            float priceRange = vp.highPrice - vp.lowPrice
            float rowHeight = priceRange / vpRows
            int timeRange = vp.endTime - vp.startTime
            
            color boxColor = vp.isBullish ? vpBullBoxColor : vpBearBoxColor
            color histColor = vp.isBullish ? vpBullHistColor : vpBearHistColor
            
            if showVpRangeBox and isFrozen
                vp.rangeBox := box.new(
                     left=vp.startTime,
                     top=vp.highPrice,
                     right=vp.endTime,
                     bottom=vp.lowPrice,
                     xloc=xloc.bar_time,
                     bgcolor=color.new(boxColor, vpBoxBgTransparency),
                     border_color=color.new(boxColor, 30),
                     border_width=vpBoxBorderWidth,
                     border_style=line.style_solid)
            
            if showDevelopingVABox and useDevelopingVA and isFrozen and not na(vp.vah) and not na(vp.val)
                vp.developingVABox := box.new(
                     left=vp.startTime,
                     top=vp.vah,
                     right=vp.endTime,
                     bottom=vp.val,
                     xloc=xloc.bar_time,
                     bgcolor=color.new(developingVAColor, developingVATransp),
                     border_color=color.new(developingVAColor, 60),
                     border_width=1,
                     border_style=line.style_dashed)
            
            if showVpBoxLabels and showVpRangeBox and isFrozen
                int barCount = vp.endBar - vp.startBar
                
                int labelX = vp.startTime
                float labelY = vp.highPrice
                string labelStyle = label.style_label_down
                
                if vpBoxLabelPosition == 'Top Right'
                    labelX := vp.endTime
                    labelY := vp.highPrice
                    labelStyle := label.style_label_down
                else if vpBoxLabelPosition == 'Top Left'
                    labelX := vp.startTime
                    labelY := vp.highPrice
                    labelStyle := label.style_label_down
                else if vpBoxLabelPosition == 'Bottom Right'
                    labelX := vp.endTime
                    labelY := vp.lowPrice
                    labelStyle := label.style_label_up
                else
                    labelX := vp.startTime
                    labelY := vp.lowPrice
                    labelStyle := label.style_label_up
                
                string labelText = ''
                
                if showLabelTitle
                    labelText += '📊 CHoCH ' + (vp.isBullish ? '🟢' : '🔴') + '\n'
                
                if showLabelBars
                    labelText += '⏱ Bars: ' + str.tostring(barCount) + '\n'
                
                if showLabelRange
                    labelText += '📈 Range: ' + str.tostring(priceRange, '#.##') + '\n'
                
                if showLabelPOC and not na(vp.poc)
                    labelText += '🎯 POC: ' + str.tostring(vp.poc, '#.##') + '\n'
                
                if showLabelVAH and not na(vp.vah)
                    labelText += '🔼 VAH: ' + str.tostring(vp.vah, '#.##') + '\n'
                
                if showLabelVAL and not na(vp.val)
                    labelText += '🔽 VAL: ' + str.tostring(vp.val, '#.##')
                
                if str.length(labelText) > 0
                    vp.infoLabel := label.new(
                         x=labelX,
                         y=labelY,
                         text=labelText,
                         xloc=xloc.bar_time,
                         color=color.new(boxColor, 70),
                         textcolor=color.white,
                         style=labelStyle,
                         size=getLabelSize(vpBoxLabelSize))
            
            if showVpHistogram
                for row = 0 to vpRows - 1
                    float vol = vp.volumes.get(row)
                    if vol > 0
                        float price = vp.lowPrice + (row + 0.5) * rowHeight
                        float volRatio = vol / maxVolume
                        
                        int barLeftTime = vp.startTime
                        int barRightTime = vp.endTime
                        
                        if vpHistAlignment == 'Right Aligned'
                            int maxBarWidth = int(timeRange * (vpMaxWidth / 100))
                            int barWidth = int(maxBarWidth * volRatio)
                            barLeftTime := vp.endTime - barWidth
                            barRightTime := vp.endTime
                        else
                            int barWidth = int(timeRange * volRatio)
                            barLeftTime := vp.startTime
                            barRightTime := vp.startTime + barWidth
                        
                        if barRightTime > barLeftTime
                            float barTopHeight = rowHeight / 2 * vpBarSpacing
                            float barBottomHeight = rowHeight / 2 * vpBarSpacing
                            
                            if vpHistStyle == 'Filled Bars'
                                box histBox = box.new(
                                     left=barLeftTime,
                                     top=price + barTopHeight,
                                     right=barRightTime,
                                     bottom=price - barBottomHeight,
                                     xloc=xloc.bar_time,
                                     bgcolor=color.new(histColor, vpHistTransparency),
                                     border_color=color.new(histColor, vpHistTransparency - 20),
                                     border_width=1)
                                vp.histogramBoxes.push(histBox)
            
            if showPOC and not na(vp.poc)
                vp.pocLine := line.new(
                     x1=vp.startTime, y1=vp.poc,
                     x2=vp.endTime, y2=vp.poc,
                     xloc=xloc.bar_time,
                     color=pocColor,
                     width=pocWidth,
                     style=pocStyle)
            
            if showVAH and not na(vp.vah)
                vp.vahLine := line.new(
                     x1=vp.startTime, y1=vp.vah,
                     x2=vp.endTime, y2=vp.vah,
                     xloc=xloc.bar_time,
                     color=vahColor,
                     width=vahWidth,
                     style=vahStyle)
            
            if showVAL and not na(vp.val)
                vp.valLine := line.new(
                     x1=vp.startTime, y1=vp.val,
                     x2=vp.endTime, y2=vp.val,
                     xloc=xloc.bar_time,
                     color=valColor,
                     width=valWidth,
                     style=valStyle)

//═════════════════════════════════════════════════════════════════════════════════
// SWING HIGH
//═════════════════════════════════════════════════════════════════════════════════

if isSwingHigh
    float val = high[lookback]
    int bar = bar_index - lookback
    line l = line.new(x1=bar, y1=val, x2=bar, y2=val, color=highColor, width=highWidth, style=highStyle)
    
    array.push(swingHighLines, l)
    array.push(swingHighVals, val)
    array.push(swingHighBars, bar)
    array.push(swingHighActive, true)
    
    if array.size(swingHighLines) > maxSwingsHigh
        line.delete(array.shift(swingHighLines))
        array.shift(swingHighVals)
        array.shift(swingHighBars)
        array.shift(swingHighActive)

//═════════════════════════════════════════════════════════════════════════════════
// SWING LOW
//═════════════════════════════════════════════════════════════════════════════════

if isSwingLow
    float val = low[lookback]
    int bar = bar_index - lookback
    line l = line.new(x1=bar, y1=val, x2=bar, y2=val, color=lowColor, width=lowWidth, style=lowStyle)
    
    array.push(swingLowLines, l)
    array.push(swingLowVals, val)
    array.push(swingLowBars, bar)
    array.push(swingLowActive, true)
    
    if array.size(swingLowLines) > maxSwingsLow
        line.delete(array.shift(swingLowLines))
        array.shift(swingLowVals)
        array.shift(swingLowBars)
        array.shift(swingLowActive)

//═════════════════════════════════════════════════════════════════════════════════
// HIGH BREAKS
//═════════════════════════════════════════════════════════════════════════════════

if array.size(swingHighLines) > 0
    for i = 0 to array.size(swingHighLines) - 1
        if array.get(swingHighActive, i)
            int x1 = array.get(swingHighBars, i)
            float y = array.get(swingHighVals, i)
            line l = array.get(swingHighLines, i)
            
            bool closeAboveSwing = close > y
            bool printAboveSwing = high > y
            bool broken = closeAboveSwing and printAboveSwing
            
            if broken
                array.set(swingHighActive, i, false)
                line.set_color(l, highFrozenColor)
                line.set_width(l, highFrozenWidth)
                line.set_style(l, highFrozenStyle)
                line.set_xy2(l, bar_index, y)
                
                bool isChoch = currentTrend == -1
                
                if isChoch
                    int labelBar = chochLabelPosition == 'Center of Line' ? int((x1 + bar_index) / 2) : bar_index
                    label.new(labelBar, y, 'CHoCH ↑',
                         color=color.new(color.white, 100),
                         textcolor=chochUpColor,
                         style=label.style_none,
                         yloc=yloc.price,
                         textalign=text.align_center,
                         size=getLabelSize(chochLabelSize))
                    
                    // Start VWAP (NEU!)
                    if enableVWAP
                        vwapAnchor := bar_index
                        vwapActive := true
                        vwapBullish := true
                else
                    color currentBosColor = currentTrend == 1 ? bosUpColor : bosDownColor
                    
                    int labelBar = bosLabelPosition == 'Center of Line' ? int((x1 + bar_index) / 2) : bar_index
                    label.new(labelBar, y, 'BOS ↑',
                         color=color.new(color.white, 100),
                         textcolor=currentBosColor,
                         style=label.style_none,
                         yloc=yloc.price,
                         textalign=text.align_center,
                         size=getLabelSize(bosLabelSize))
                    
                    if showBosLabels
                        int barCount = bar_index - x1
                        float swingRange = y - low[bar_index - x1]
                        
                        int bosLabelX = x1
                        float bosLabelY = y
                        string bosLabelStyle = label.style_label_down
                        
                        if bosInfoPosition == 'Top Right'
                            bosLabelX := bar_index
                            bosLabelY := y
                            bosLabelStyle := label.style_label_down
                        else if bosInfoPosition == 'Top Left'
                            bosLabelX := x1
                            bosLabelY := y
                            bosLabelStyle := label.style_label_down
                        else if bosInfoPosition == 'Bottom Right'
                            bosLabelX := bar_index
                            bosLabelY := y
                            bosLabelStyle := label.style_label_up
                        else
                            bosLabelX := x1
                            bosLabelY := y
                            bosLabelStyle := label.style_label_up
                        
                        string bosText = '🔄 BOS\n'
                        
                        if showBosInfoBars
                            bosText += '⏱ ' + str.tostring(barCount) + ' bars\n'
                        
                        if showBosInfoRange
                            bosText += '📏 ' + str.tostring(swingRange, '#.##')
                        
                        label.new(
                             x=bosLabelX,
                             y=bosLabelY,
                             text=bosText,
                             xloc=xloc.bar_index,
                             color=color.new(currentBosColor, 70),
                             textcolor=color.white,
                             style=bosLabelStyle,
                             size=getLabelSize(bosInfoSize))
                
                if isChoch
                    if enableVP and showFrozenVP and not na(chochStartBar)
                        VolumeProfileData vpData = calculateVolumeProfile(
                             chochStartBar, bar_index, 
                             chochStartTime, time)
                        
                        drawVolumeProfile(vpData, true)
                        array.push(volumeProfiles, vpData)
                        
                        if array.size(volumeProfiles) > maxVPBoxes
                            old = array.shift(volumeProfiles)
                            if not na(old.rangeBox)
                                box.delete(old.rangeBox)
                            if not na(old.pocLine)
                                line.delete(old.pocLine)
                            if not na(old.vahLine)
                                line.delete(old.vahLine)
                            if not na(old.valLine)
                                line.delete(old.valLine)
                            if not na(old.developingVABox)
                                box.delete(old.developingVABox)
                            if not na(old.infoLabel)
                                label.delete(old.infoLabel)
                            for b in old.histogramBoxes
                                box.delete(b)
                    
                    if not na(currentVpBox)
                        box.delete(currentVpBox)
                        currentVpBox := na
                    if not na(currentPocLine)
                        line.delete(currentPocLine)
                        currentPocLine := na
                    if not na(currentVahLine)
                        line.delete(currentVahLine)
                        currentVahLine := na
                    if not na(currentValLine)
                        line.delete(currentValLine)
                        currentValLine := na
                    if not na(currentDevelopingVABox)
                        box.delete(currentDevelopingVABox)
                        currentDevelopingVABox := na
                    if not na(currentVpLabel)
                        label.delete(currentVpLabel)
                        currentVpLabel := na
                    
                    if not na(anchoredVAH)
                        line.delete(anchoredVAH)
                        anchoredVAH := na
                    if not na(anchoredVAL)
                        line.delete(anchoredVAL)
                        anchoredVAL := na
                    if not na(anchoredVALabel)
                        label.delete(anchoredVALabel)
                        anchoredVALabel := na
                    
                    array.clear(cumulativePrices)
                    array.clear(cumulativeVolumes)
                    
                    chochStartBar := bar_index
                    chochStartTime := time
                    currentTrend := 1
                else
                    currentTrend := 1
            else
                line.set_xy2(l, bar_index, y)

//═════════════════════════════════════════════════════════════════════════════════
// LOW BREAKS
//═════════════════════════════════════════════════════════════════════════════════

if array.size(swingLowLines) > 0
    for i = 0 to array.size(swingLowLines) - 1
        if array.get(swingLowActive, i)
            int x1 = array.get(swingLowBars, i)
            float y = array.get(swingLowVals, i)
            line l = array.get(swingLowLines, i)
            
            bool closeBelowSwing = close < y
            bool printBelowSwing = low < y
            bool broken = closeBelowSwing and printBelowSwing
            
            if broken
                array.set(swingLowActive, i, false)
                line.set_color(l, lowFrozenColor)
                line.set_width(l, lowFrozenWidth)
                line.set_style(l, lowFrozenStyle)
                line.set_xy2(l, bar_index, y)
                
                bool isChoch = currentTrend == 1
                
                if isChoch
                    int labelBar = chochLabelPosition == 'Center of Line' ? int((x1 + bar_index) / 2) : bar_index
                    label.new(labelBar, y, 'CHoCH ↓',
                         color=color.new(color.white, 100),
                         textcolor=chochDownColor,
                         style=label.style_none,
                         yloc=yloc.price,
                         textalign=text.align_center,
                         size=getLabelSize(chochLabelSize))
                    
                    // Start VWAP (NEU!)
                    if enableVWAP
                        vwapAnchor := bar_index
                        vwapActive := true
                        vwapBullish := false
                else
                    color currentBosColor = currentTrend == 1 ? bosUpColor : bosDownColor
                    
                    int labelBar = bosLabelPosition == 'Center of Line' ? int((x1 + bar_index) / 2) : bar_index
                    label.new(labelBar, y, 'BOS ↓',
                         color=color.new(color.white, 100),
                         textcolor=currentBosColor,
                         style=label.style_none,
                         yloc=yloc.price,
                         textalign=text.align_center,
                         size=getLabelSize(bosLabelSize))
                    
                    if showBosLabels
                        int barCount = bar_index - x1
                        float swingRange = high[bar_index - x1] - y
                        
                        int bosLabelX = x1
                        float bosLabelY = y
                        string bosLabelStyle = label.style_label_up
                        
                        if bosInfoPosition == 'Top Right'
                            bosLabelX := bar_index
                            bosLabelY := y
                            bosLabelStyle := label.style_label_down
                        else if bosInfoPosition == 'Top Left'
                            bosLabelX := x1
                            bosLabelY := y
                            bosLabelStyle := label.style_label_down
                        else if bosInfoPosition == 'Bottom Right'
                            bosLabelX := bar_index
                            bosLabelY := y
                            bosLabelStyle := label.style_label_up
                        else
                            bosLabelX := x1
                            bosLabelY := y
                            bosLabelStyle := label.style_label_up
                        
                        string bosText = '🔄 BOS\n'
                        
                        if showBosInfoBars
                            bosText += '⏱ ' + str.tostring(barCount) + ' bars\n'
                        
                        if showBosInfoRange
                            bosText += '📏 ' + str.tostring(swingRange, '#.##')
                        
                        label.new(
                             x=bosLabelX,
                             y=bosLabelY,
                             text=bosText,
                             xloc=xloc.bar_index,
                             color=color.new(currentBosColor, 70),
                             textcolor=color.white,
                             style=bosLabelStyle,
                             size=getLabelSize(bosInfoSize))
                
                if isChoch
                    if enableVP and showFrozenVP and not na(chochStartBar)
                        VolumeProfileData vpData = calculateVolumeProfile(
                             chochStartBar, bar_index,
                             chochStartTime, time)
                        
                        drawVolumeProfile(vpData, true)
                        array.push(volumeProfiles, vpData)
                        
                        if array.size(volumeProfiles) > maxVPBoxes
                            old = array.shift(volumeProfiles)
                            if not na(old.rangeBox)
                                box.delete(old.rangeBox)
                            if not na(old.pocLine)
                                line.delete(old.pocLine)
                            if not na(old.vahLine)
                                line.delete(old.vahLine)
                            if not na(old.valLine)
                                line.delete(old.valLine)
                            if not na(old.developingVABox)
                                box.delete(old.developingVABox)
                            if not na(old.infoLabel)
                                label.delete(old.infoLabel)
                            for b in old.histogramBoxes
                                box.delete(b)
                    
                    if not na(currentVpBox)
                        box.delete(currentVpBox)
                        currentVpBox := na
                    if not na(currentPocLine)
                        line.delete(currentPocLine)
                        currentPocLine := na
                    if not na(currentVahLine)
                        line.delete(currentVahLine)
                        currentVahLine := na
                    if not na(currentValLine)
                        line.delete(currentValLine)
                        currentValLine := na
                    if not na(currentDevelopingVABox)
                        box.delete(currentDevelopingVABox)
                        currentDevelopingVABox := na
                    if not na(currentVpLabel)
                        label.delete(currentVpLabel)
                        currentVpLabel := na
                    
                    if not na(anchoredVAH)
                        line.delete(anchoredVAH)
                        anchoredVAH := na
                    if not na(anchoredVAL)
                        line.delete(anchoredVAL)
                        anchoredVAL := na
                    if not na(anchoredVALabel)
                        label.delete(anchoredVALabel)
                        anchoredVALabel := na
                    
                    array.clear(cumulativePrices)
                    array.clear(cumulativeVolumes)
                    
                    chochStartBar := bar_index
                    chochStartTime := time
                    currentTrend := -1
                else
                    currentTrend := -1
            else
                line.set_xy2(l, bar_index, y)

//═════════════════════════════════════════════════════════════════════════════════
// UPDATE CURRENT VP + DEVELOPING VA (DEIN ORIGINAL!)
//═════════════════════════════════════════════════════════════════════════════════

if enableVP and showCurrentVP and not na(chochStartBar)
    float currentHigh = high[bar_index - chochStartBar]
    float currentLow = low[bar_index - chochStartBar]
    float currentOpen = open[bar_index - chochStartBar]
    float currentClose = close
    
    for i = chochStartBar to bar_index
        if bar_index - i >= 0
            currentHigh := math.max(currentHigh, high[bar_index - i])
            currentLow := math.min(currentLow, low[bar_index - i])
    
    bool currentIsBullish = currentClose > currentOpen
    color currentBoxColor = currentIsBullish ? vpBullBoxColor : vpBearBoxColor
    
    if showVpRangeBox
        if na(currentVpBox)
            currentVpBox := box.new(
                 left=chochStartTime,
                 top=currentHigh,
                 right=time,
                 bottom=currentLow,
                 xloc=xloc.bar_time,
                 bgcolor=color.new(currentBoxColor, vpBoxBgTransparency),
                 border_color=color.new(currentBoxColor, 30),
                 border_width=vpBoxBorderWidth,
                 border_style=line.style_solid)
        else
            box.set_top(currentVpBox, currentHigh)
            box.set_bottom(currentVpBox, currentLow)
            box.set_right(currentVpBox, time)
            box.set_bgcolor(currentVpBox, color.new(currentBoxColor, vpBoxBgTransparency))
            box.set_border_color(currentVpBox, color.new(currentBoxColor, 30))
    
    VolumeProfileData currentVP = calculateVolumeProfile(chochStartBar, bar_index, chochStartTime, time)
    
    if showDevelopingVABox and useDevelopingVA and not na(currentVP.vah) and not na(currentVP.val)
        if na(currentDevelopingVABox)
            currentDevelopingVABox := box.new(
                 left=chochStartTime,
                 top=currentVP.vah,
                 right=time,
                 bottom=currentVP.val,
                 xloc=xloc.bar_time,
                 bgcolor=color.new(developingVAColor, developingVATransp),
                 border_color=color.new(developingVAColor, 60),
                 border_width=1,
                 border_style=line.style_dashed)
        else
            box.set_top(currentDevelopingVABox, currentVP.vah)
            box.set_bottom(currentDevelopingVABox, currentVP.val)
            box.set_right(currentDevelopingVABox, time)
    
    if showPOC and not na(currentVP.poc)
        if na(currentPocLine)
            currentPocLine := line.new(
                 x1=chochStartTime, y1=currentVP.poc,
                 x2=time, y2=currentVP.poc,
                 xloc=xloc.bar_time,
                 color=vpCurrentColor,
                 width=vpCurrentWidth,
                 style=vpCurrentStyle)
        else
            line.set_y1(currentPocLine, currentVP.poc)
            line.set_y2(currentPocLine, currentVP.poc)
            line.set_x2(currentPocLine, time)
    
    if showVAH and not na(currentVP.vah)
        if na(currentVahLine)
            currentVahLine := line.new(
                 x1=chochStartTime, y1=currentVP.vah,
                 x2=time, y2=currentVP.vah,
                 xloc=xloc.bar_time,
                 color=vahColor,
                 width=vahWidth,
                 style=vahStyle)
        else
            line.set_y1(currentVahLine, currentVP.vah)
            line.set_y2(currentVahLine, currentVP.vah)
            line.set_x2(currentVahLine, time)
    
    if showVAL and not na(currentVP.val)
        if na(currentValLine)
            currentValLine := line.new(
                 x1=chochStartTime, y1=currentVP.val,
                 x2=time, y2=currentVP.val,
                 xloc=xloc.bar_time,
                 color=valColor,
                 width=valWidth,
                 style=valStyle)
        else
            line.set_y1(currentValLine, currentVP.val)
            line.set_y2(currentValLine, currentVP.val)
            line.set_x2(currentValLine, time)

//═════════════════════════════════════════════════════════════════════════════════
// UPDATE ANCHORED VALUE AREA (DEIN ORIGINAL!)
//═════════════════════════════════════════════════════════════════════════════════

if showAnchoredVA and not na(chochStartBar)
    float rangeHigh = high[bar_index - chochStartBar]
    float rangeLow = low[bar_index - chochStartBar]
    
    for i = chochStartBar to bar_index
        if bar_index - i >= 0
            rangeHigh := math.max(rangeHigh, high[bar_index - i])
            rangeLow := math.min(rangeLow, low[bar_index - i])
    
    float priceRange = rangeHigh - rangeLow
    
    if priceRange > 0
        if array.size(cumulativePrices) == 0
            array.clear(cumulativePrices)
            array.clear(cumulativeVolumes)
            
            float rowHeight = priceRange / vpRows
            for row = 0 to vpRows - 1
                array.push(cumulativePrices, rangeLow + (row + 0.5) * rowHeight)
                array.push(cumulativeVolumes, 0.0)
        
        float rowHeight = priceRange / vpRows
        for row = 0 to vpRows - 1
            float rowPrice = rangeLow + (row + 0.5) * rowHeight
            
            if low <= rowPrice and rowPrice <= high
                if row < array.size(cumulativeVolumes)
                    float currentVol = array.get(cumulativeVolumes, row)
                    array.set(cumulativeVolumes, row, currentVol + volume)
                    array.set(cumulativePrices, row, rowPrice)
        
        float maxVol = array.max(cumulativeVolumes)
        if maxVol > 0
            int pocIdx = array.indexof(cumulativeVolumes, maxVol)
            float totalVol = array.sum(cumulativeVolumes)
            
            if totalVol > 0
                float targetVol = totalVol * (valueAreaPct / 100)
                float vaVol = array.get(cumulativeVolumes, pocIdx)
                int upperIdx = pocIdx
                int lowerIdx = pocIdx
                
                while vaVol < targetVol and (upperIdx < vpRows - 1 or lowerIdx > 0)
                    float upperVol = upperIdx < vpRows - 1 ? array.get(cumulativeVolumes, upperIdx + 1) : 0
                    float lowerVol = lowerIdx > 0 ? array.get(cumulativeVolumes, lowerIdx - 1) : 0
                    
                    if upperVol > lowerVol and upperIdx < vpRows - 1
                        upperIdx += 1
                        vaVol += upperVol
                    else if lowerIdx > 0
                        lowerIdx -= 1
                        vaVol += lowerVol
                    else
                        break
                
                float anchoredVAHPrice = array.get(cumulativePrices, upperIdx)
                float anchoredVALPrice = array.get(cumulativePrices, lowerIdx)
                
                if na(anchoredVAH)
                    anchoredVAH := line.new(
                         x1=chochStartTime, y1=anchoredVAHPrice,
                         x2=time, y2=anchoredVAHPrice,
                         xloc=xloc.bar_time,
                         color=color.new(anchoredVAColor, anchoredVATransp),
                         width=anchoredVAWidth,
                         style=anchoredVALineStyle)
                else
                    line.set_y1(anchoredVAH, anchoredVAHPrice)
                    line.set_y2(anchoredVAH, anchoredVAHPrice)
                    line.set_x2(anchoredVAH, time)
                
                if na(anchoredVAL)
                    anchoredVAL := line.new(
                         x1=chochStartTime, y1=anchoredVALPrice,
                         x2=time, y2=anchoredVALPrice,
                         xloc=xloc.bar_time,
                         color=color.new(anchoredVAColor, anchoredVATransp),
                         width=anchoredVAWidth,
                         style=anchoredVALineStyle)
                else
                    line.set_y1(anchoredVAL, anchoredVALPrice)
                    line.set_y2(anchoredVAL, anchoredVALPrice)
                    line.set_x2(anchoredVAL, time)

//═════════════════════════════════════════════════════════════════════════════════
// PLOT VWAP
//═════════════════════════════════════════════════════════════════════════════════

[vwap, stdDev] = calculateVWAP()

vwapColor = enableVWAP and vwapActive ? (vwapBullish ? vwapBullColor : vwapBearColor) : na
plot(vwap, "VWAP", vwapColor, vwapWidth)

// Standard Deviation Bands
vwapStd1Up = enableVWAP and showVwapStd1 and vwapActive and not na(vwap) ? vwap + stdDev : na
vwapStd1Dn = enableVWAP and showVwapStd1 and vwapActive and not na(vwap) ? vwap - stdDev : na

p1up = plot(vwapStd1Up, "±1σ Up", vwapStd1Color, 1)
p1dn = plot(vwapStd1Dn, "±1σ Dn", vwapStd1Color, 1)

vwapStd2Up = enableVWAP and showVwapStd2 and vwapActive and not na(vwap) ? vwap + stdDev * 2 : na
vwapStd2Dn = enableVWAP and showVwapStd2 and vwapActive and not na(vwap) ? vwap - stdDev * 2 : na

p2up = plot(vwapStd2Up, "±2σ Up", vwapStd2Color, 1)
p2dn = plot(vwapStd2Dn, "±2σ Dn", vwapStd2Color, 1)

vwapStd3Up = enableVWAP and showVwapStd3 and vwapActive and not na(vwap) ? vwap + stdDev * 3 : na
vwapStd3Dn = enableVWAP and showVwapStd3 and vwapActive and not na(vwap) ? vwap - stdDev * 3 : na

plot(vwapStd3Up, "±3σ Up", vwapStd3Color, 1)
plot(vwapStd3Dn, "±3σ Dn", vwapStd3Color, 1)

// Fills
vwapFillCol = enableVWAP and showVwapFill and vwapActive ? 
     color.new(vwapBullish ? color.green : color.red, vwapFillAlpha) : na

fill(p1up, p1dn, vwapFillCol)
fill(p2up, p2dn, color.new(vwapFillCol, vwapFillAlpha + 2))

//═════════════════════════════════════════════════════════════════════════════════
// DEBUG TABLE (ERWEITERT!)
//═════════════════════════════════════════════════════════════════════════════════

var table debugTable = table.new(position.top_right, 2, 7, bgcolor=color.new(color.black, 85))

if barstate.islast
    table.cell(debugTable, 0, 0, 'Trend:', text_color=color.white)
    table.cell(debugTable, 1, 0, 
         currentTrend == 1 ? '▲ Bull' : currentTrend == -1 ? '▼ Bear' : '—',
         text_color=currentTrend == 1 ? color.green : currentTrend == -1 ? color.red : color.gray)
    
    table.cell(debugTable, 0, 1, 'VP System:', text_color=color.white)
    table.cell(debugTable, 1, 1, enableVP ? 'ON ✅' : 'OFF ❌',
         text_color=enableVP ? color.lime : color.red)
    
    table.cell(debugTable, 0, 2, 'Frozen VPs:', text_color=color.white)
    table.cell(debugTable, 1, 2, str.tostring(array.size(volumeProfiles)) + '/' + str.tostring(maxVPBoxes), 
         text_color=color.yellow)
    
    table.cell(debugTable, 0, 3, 'Histograms:', text_color=color.white)
    table.cell(debugTable, 1, 3, showVpHistogram ? vpHistAlignment : 'OFF',
         text_color=showVpHistogram ? color.aqua : color.gray)
    
    table.cell(debugTable, 0, 4, 'Anchored VA:', text_color=color.white)
    table.cell(debugTable, 1, 4, na(anchoredVAH) ? 'Waiting' : 'ACTIVE 🟣',
         text_color=na(anchoredVAH) ? color.gray : color.purple)
    
    // VWAP Info (NEU!)
    table.cell(debugTable, 0, 5, 'VWAP:', text_color=color.white)
    vwapText = enableVWAP ? (vwapActive ? str.tostring(vwap, "#.##") : 'Waiting') : 'OFF'
    vwapCol = enableVWAP and vwapActive ? (vwapBullish ? color.lime : color.red) : color.gray
    table.cell(debugTable, 1, 5, vwapText, text_color=vwapCol)
    
    table.cell(debugTable, 0, 6, 'VWAP StdDev:', text_color=color.white)
    stdText = enableVWAP and vwapActive and not na(stdDev) ? str.tostring(stdDev, "#.##") + 'σ' : 'N/A'
    table.cell(debugTable, 1, 6, stdText, text_color=color.orange)-VP-VA