# 🎨 Big Trades Bubble Visualization - Design Spec

## 📐 Bubble Design (wie Bookmap/FaberVaale)

### Anatomie einer Big Trade Bubble

```
     ╭─────╮
    ╱  150  ╲     ← Volume Label (optional)
   │   BTC   │
    ╲       ╱
     ╰─────╯
       
  [Glanz-Effekt]   ← Oben links: weißer Glanz
  [Farb-Füllung]   ← Grün = Buy, Rot = Sell
  [Schatten]       ← Unten rechts: grauer Schatten (3D)
  [Border]         ← Dickerer Rand zur Betonung
```

---

## 🎨 Farb-Schema

### Größe & Transparenz nach Volumen

| Volume Ratio | Bubble Size | Alpha | Verwendung |
|--------------|-------------|-------|------------|
| 1.0x - 1.5x  | 15px        | 180   | Normale Big Trades |
| 1.5x - 2.0x  | 20px        | 200   | Größere Trades |
| 2.0x - 3.0x  | 30px        | 220   | Sehr große Trades |
| 3.0x+        | 40-50px     | 240   | Whale Trades 🐋 |

### Farben

```csharp
// BUY SIDE (Aggressive Buying)
Color.FromArgb(180, 0, 255, 0)      // Grün, leicht transparent
Color.FromArgb(220, 0, 255, 0)      // Grün, weniger transparent (große Trades)
Color.FromArgb(100, 0, 200, 0)      // Dunkelgrün für Schatten

// SELL SIDE (Aggressive Selling)
Color.FromArgb(180, 255, 0, 0)      // Rot, leicht transparent
Color.FromArgb(220, 255, 0, 0)      // Rot, weniger transparent (große Trades)
Color.FromArgb(100, 200, 0, 0)      // Dunkelrot für Schatten

// NEUTRAL (wenn Delta ausgeglichen)
Color.FromArgb(180, 255, 255, 0)    // Gelb
```

---

## 📊 Implementierungs-Beispiele

### Beispiel 1: Bookmap Style

```csharp
private void DrawBookmapStyleBubble(RenderContext context, BigTrade trade)
{
    int x = ChartInfo.GetXByBar(trade.Bar);
    int y = ChartInfo.GetYByPrice(trade.Price);
    
    float size = CalculateBubbleSize(trade.Volume);
    
    // Layer 1: Schatten (unten rechts versetzt)
    using (var shadowBrush = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
    {
        context.Graphics.FillEllipse(shadowBrush,
            x - size/2 + 3, y - size/2 + 3, size, size);
    }
    
    // Layer 2: Hauptkreis mit Farbverlauf
    using (var path = new GraphicsPath())
    {
        path.AddEllipse(x - size/2, y - size/2, size, size);
        
        using (var brush = new PathGradientBrush(path))
        {
            brush.CenterColor = trade.IsBuy 
                ? Color.FromArgb(220, 100, 255, 100)  // Hellgrün in Mitte
                : Color.FromArgb(220, 255, 100, 100); // Hellrot in Mitte
            
            brush.SurroundColors = new Color[] { 
                trade.IsBuy 
                    ? Color.FromArgb(180, 0, 200, 0)   // Dunkelgrün außen
                    : Color.FromArgb(180, 200, 0, 0)   // Dunkelrot außen
            };
            
            context.Graphics.FillEllipse(brush, x - size/2, y - size/2, size, size);
        }
    }
    
    // Layer 3: Glanz-Effekt (oben links)
    using (var glowBrush = new SolidBrush(Color.FromArgb(120, 255, 255, 255)))
    {
        float glowSize = size * 0.35f;
        context.Graphics.FillEllipse(glowBrush,
            x - size/3, y - size/3, glowSize, glowSize);
    }
    
    // Layer 4: Border
    using (var borderPen = new Pen(
        trade.IsBuy ? Color.FromArgb(255, 0, 255, 0) : Color.FromArgb(255, 255, 0, 0), 
        2))
    {
        context.Graphics.DrawEllipse(borderPen, x - size/2, y - size/2, size, size);
    }
    
    // Layer 5: Volume Text
    DrawVolumeLabel(context, x, y, trade.Volume, size);
}
```

### Beispiel 2: FaberVaale Style (minimalistischer)

```csharp
private void DrawFaberVaaleStyleBubble(RenderContext context, BigTrade trade)
{
    int x = ChartInfo.GetXByBar(trade.Bar);
    int y = ChartInfo.GetYByPrice(trade.Price);
    
    float size = CalculateBubbleSize(trade.Volume);
    
    // Einfacher gefüllter Kreis, keine Schatten
    Color mainColor = trade.IsBuy 
        ? Color.FromArgb(200, 0, 255, 0)
        : Color.FromArgb(200, 255, 0, 0);
    
    using (var brush = new SolidBrush(mainColor))
    {
        context.Graphics.FillEllipse(brush, x - size/2, y - size/2, size, size);
    }
    
    // Dünner weißer Border
    using (var borderPen = new Pen(Color.White, 1))
    {
        context.Graphics.DrawEllipse(borderPen, x - size/2, y - size/2, size, size);
    }
    
    // Volumen als Badge (oben rechts)
    if (ShowBigTradeVolume && size > 20)
    {
        DrawVolumeBadge(context, x + size/2, y - size/2, trade.Volume);
    }
}
```

### Beispiel 3: DeepCharts Style (mit Pulsation)

```csharp
private Dictionary<BigTrade, float> _pulsationPhase = new Dictionary<BigTrade, float>();

private void DrawDeepChartsStyleBubble(RenderContext context, BigTrade trade)
{
    int x = ChartInfo.GetXByBar(trade.Bar);
    int y = ChartInfo.GetYByPrice(trade.Price);
    
    float baseSize = CalculateBubbleSize(trade.Volume);
    
    // Pulsations-Effekt (für neue Trades)
    if (!_pulsationPhase.ContainsKey(trade))
        _pulsationPhase[trade] = 0;
    
    float pulse = (float)Math.Sin(_pulsationPhase[trade]) * 0.2f;
    float size = baseSize * (1 + pulse);
    
    _pulsationPhase[trade] += 0.1f; // Erhöhe Phase
    
    // Äußerer Ring (Pulsation)
    Color ringColor = trade.IsBuy 
        ? Color.FromArgb(80, 0, 255, 0)
        : Color.FromArgb(80, 255, 0, 0);
    
    using (var ringPen = new Pen(ringColor, 3))
    {
        float ringSize = size * 1.3f;
        context.Graphics.DrawEllipse(ringPen, 
            x - ringSize/2, y - ringSize/2, ringSize, ringSize);
    }
    
    // Innerer Kreis
    Color fillColor = trade.IsBuy 
        ? Color.FromArgb(220, 0, 255, 0)
        : Color.FromArgb(220, 255, 0, 0);
    
    using (var brush = new SolidBrush(fillColor))
    {
        context.Graphics.FillEllipse(brush, x - size/2, y - size/2, size, size);
    }
    
    // Icon im Zentrum (↑ für Buy, ↓ für Sell)
    DrawTradeDirectionIcon(context, x, y, trade.IsBuy, size);
}
```

---

## 🎯 Size Calculation

```csharp
private float CalculateBubbleSize(decimal volume)
{
    // Basis-Größe
    float minSize = 10f;
    float maxSize = 50f;
    
    // Volumen-Verhältnis
    float volumeRatio = (float)(volume / BigTradeMinVolume);
    
    // Logarithmische Skalierung für große Werte
    float logScale = (float)Math.Log10(volumeRatio + 1) * 15f;
    
    // Clamp auf Min/Max
    float size = Math.Min(Math.Max(minSize + logScale, minSize), maxSize);
    
    return size;
}
```

---

## 📍 Positioning

### Option 1: At Price Level (Standard)
```csharp
int y = ChartInfo.GetYByPrice(trade.Price);
```

### Option 2: At High/Low
```csharp
int y = trade.IsBuy 
    ? ChartInfo.GetYByPrice(GetCandle(trade.Bar).High)
    : ChartInfo.GetYByPrice(GetCandle(trade.Bar).Low);
```

### Option 3: With Offset (bei Überlappung)
```csharp
int y = ChartInfo.GetYByPrice(trade.Price);
int offset = GetOverlapOffset(trade); // Prüfe andere Trades
y += offset;
```

---

## 🎨 Fortgeschrittene Features

### 1. Volume Label Formatting

```csharp
private string FormatVolume(decimal volume)
{
    if (volume >= 1_000_000)
        return (volume / 1_000_000m).ToString("F1") + "M";
    else if (volume >= 1_000)
        return (volume / 1_000m).ToString("F1") + "K";
    else
        return volume.ToString("F0");
}
```

### 2. Time-based Fade Out

```csharp
private int GetBubbleAlpha(BigTrade trade, int currentBar)
{
    int barAge = currentBar - trade.Bar;
    int maxAge = 50; // Fade after 50 bars
    
    if (barAge > maxAge)
        return 0; // Unsichtbar
    
    float fadeRatio = 1.0f - ((float)barAge / maxAge);
    return (int)(220 * fadeRatio);
}
```

### 3. Clustering Detection

```csharp
private bool IsPartOfCluster(BigTrade trade)
{
    int nearbyCount = _bigTrades.Count(t => 
        Math.Abs(t.Bar - trade.Bar) <= 3 &&
        Math.Abs(t.Price - trade.Price) <= ChartInfo.PriceChartContainer.Step * 2
    );
    
    return nearbyCount >= 3;
}
```

### 4. Cluster Visualization

```csharp
private void DrawClusterBubble(RenderContext context, List<BigTrade> cluster)
{
    // Berechne Durchschnittspreis & -position
    decimal avgPrice = cluster.Average(t => t.Price);
    int avgBar = (int)cluster.Average(t => t.Bar);
    decimal totalVolume = cluster.Sum(t => t.Volume);
    
    // Zeichne größere Bubble für Cluster
    int x = ChartInfo.GetXByBar(avgBar);
    int y = ChartInfo.GetYByPrice(avgPrice);
    
    float size = CalculateBubbleSize(totalVolume);
    
    // Mehrfarbiger Rand für gemischtes Cluster
    bool isMixed = cluster.Any(t => t.IsBuy) && cluster.Any(t => !t.IsBuy);
    
    if (isMixed)
    {
        // Zeichne gespaltenen Kreis (halb grün, halb rot)
        DrawSplitCircle(context, x, y, size, cluster);
    }
    else
    {
        // Normale Bubble, aber größer
        DrawBookmapStyleBubble(context, cluster[0]); // Verwende ersten Trade als Vorlage
    }
    
    // Cluster-Badge
    DrawClusterBadge(context, x, y - size/2, cluster.Count);
}
```

---

## 🎯 Best Practices

### Performance
- ✅ Nur sichtbare Bubbles zeichnen
- ✅ Alte Bubbles nach X Bars entfernen
- ✅ Clustering verwenden bei vielen Trades

### Usability
- ✅ Tooltips mit Trade-Details
- ✅ Klickbare Bubbles für mehr Info
- ✅ Farbschema konfigurierbar

### Visual Clarity
- ✅ Nicht zu viele Bubbles gleichzeitig
- ✅ Transparenz bei Überlappung erhöhen
- ✅ Mindestgröße für Lesbarkeit

---

## 📊 Beispiel-Output

```
Chart Visualization:

Price
42,500 ─────🟢(250)───────────────────────
                                    🔴(180)
42,400 ────────────🟢(120)─────────────────
                              🟢(90)
42,300 ───🔴(200)──────────────────────────
                        🔴(150)
42,200 ─────────────────────────────🟢(300)
       ↑         ↑         ↑         ↑
      Bar1     Bar2     Bar3     Bar4

Legende:
🟢 = Big Buy Trade
🔴 = Big Sell Trade
(Zahl) = Volumen in BTC
```

---

## 🔧 Konfigurations-Template

```csharp
// In ATAS Settings Panel:
[Display(GroupName = "💰 Big Trades Bubbles", Order = 1)]
public enum BubbleStyle
{
    Bookmap,        // Mit Schatten & Glanz
    FaberVaale,     // Minimalistisch
    DeepCharts,     // Mit Pulsation
    Custom          // Eigener Style
}

[Display(GroupName = "💰 Big Trades Bubbles", Order = 2)]
public BubbleStyle StyleMode { get; set; } = BubbleStyle.Bookmap;

[Display(GroupName = "💰 Big Trades Bubbles", Order = 3)]
public bool ShowCluster { get; set; } = true;

[Display(GroupName = "💰 Big Trades Bubbles", Order = 4)]
public bool EnablePulsation { get; set; } = false;

[Display(GroupName = "💰 Big Trades Bubbles", Order = 5)]
public bool FadeOverTime { get; set; } = true;
```

---

**Ready to implement! 🚀**

Diese Bubble-Visualization macht Big Trades sofort sichtbar und hilft dir, 
institutionelle Orders in Echtzeit zu erkennen - genau wie bei Bookmap & Co!
