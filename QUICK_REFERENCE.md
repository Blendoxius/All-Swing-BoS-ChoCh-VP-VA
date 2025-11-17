# ⚡ QUICK REFERENCE - Auf einen Blick

## 🎯 Visual Studio: Was genau anklicken?

```
START
  │
  ├─→ ✨ "Neues Projekt erstellen"
  │
  ├─→ 📚 "Klassenbibliothek (.NET Framework)"  [NICHT: Klassenbibliothek ohne Framework!]
  │
  ├─→ Projektname: "ATAS.Indicators.Custom"
  │
  ├─→ Framework: ".NET Framework 4.8"
  │
  └─→ [Erstellen]

REFERENZEN HINZUFÜGEN
  │
  ├─→ Rechtsklick auf "Verweise" im Projektmappen-Explorer
  │
  ├─→ "Verweis hinzufügen..."
  │
  ├─→ "Durchsuchen" → [Durchsuchen...]
  │
  ├─→ Navigiere zu: C:\Program Files\ATAS Platform\
  │
  ├─→ Markiere (mit Strg):
  │   ✅ ATAS.Indicators.dll
  │   ✅ OFT.Rendering.dll
  │   ✅ OFT.Attributes.dll
  │
  ├─→ [Hinzufügen]
  │
  └─→ Assemblys → Framework → System.ComponentModel.DataAnnotations [Häkchen]

CODE HINZUFÜGEN
  │
  ├─→ Class1.cs löschen (Rechtsklick → Löschen)
  │
  ├─→ Rechtsklick auf Projekt → Hinzufügen → Neues Element
  │
  ├─→ "Klasse" auswählen
  │
  ├─→ Name: "ATAS_SwingBOS_VWAP_BigTrades.cs"
  │
  ├─→ Alten Code löschen (Strg+A, Entf)
  │
  └─→ Neuen Code einfügen (Strg+V)

KOMPILIEREN
  │
  ├─→ Menü: "Build" → "Projektmappe erstellen"
  │
  ├─→ ODER: Drücke F6
  │
  └─→ DLL liegt in: bin\Debug\ATAS.Indicators.Custom.dll

IN ATAS LADEN
  │
  ├─→ ATAS öffnen
  │
  ├─→ Settings (F11) → Custom Indicators
  │
  ├─→ [Add...] → DLL auswählen
  │
  ├─→ ATAS NEU STARTEN!
  │
  └─→ Rechtsklick auf Chart → Add Indicator → "Swing BOS/CHoCH + VWAP + Big Trades"

FERTIG! 🎉
```

---

## ⚙️ Settings - Die wichtigsten:

```
📊 MAIN SETTINGS
   Swing Lookback........: 1-50   (Default: 2)  ← KANN AUF 1!

💰 BIG TRADES
   Enable Big Trades.....: ☑
   Min Volume Threshold..: 1-10000 (Default: 50) ← FREI EINSTELLBAR!
   Bubble Size...........: 5-50    (Default: 15)
   Show Volume Label.....: ☑

📈 VWAP
   Enable VWAP System....: ☑
   Show StdDev Bands.....: ☑
   Bullish Color.........: Grün
   Bearish Color.........: Rot

🔄 BOS/CHoCH
   BOS Up Color..........: Grün
   BOS Down Color........: Rot
   CHoCH Up Color........: Cyan
   CHoCH Down Color......: Orange

📦 VOLUME PROFILE
   Enable VP.............: ☑
   Show POC..............: ☑
   Show Value Area.......: ☑
   Value Area %..........: 70
```

---

## 🎨 Was du auf dem Chart siehst:

```
ELEMENTE:

🔴 Rote horizontale Linien     = Swing Highs (aktiv/gebrochen)
🟢 Grüne horizontale Linien    = Swing Lows (aktiv/gebrochen)

📊 Labels auf Linien:
   "BOS ↑" / "BOS ↓"           = Break of Structure (Trend-Fortsetzung)
   "CHoCH ↑" / "CHoCH ↓"       = Change of Character (Trendwechsel)

📈 VWAP-System:
   Farbige Linie (Grün/Rot)    = VWAP
   Blaue Bänder                = ±1σ, ±2σ Standard Deviation
   Transparente Füllung        = Zwischen Bändern

🟢🔴 Runde Kugeln/Bubbles:
   🟢 Grüne Bubble             = Big Buy Trade
   🔴 Rote Bubble              = Big Sell Trade
   Größe                       = Proportional zum Volumen
   Text in Bubble              = Volumen (z.B. "150 BTC")

📦 Volume Profile:
   Rechtecke (blau/rot)        = Range Box
   Orange Linie                = POC (Point of Control)
   Blaue gestrichelte Linien   = VAH/VAL (Value Area High/Low)
   Lila Box                    = Developing Value Area (live)
   Label oben rechts           = VP Info (Bars, Range, POC, VAH, VAL)

🟣 Lila horizontale Linien:
   Anchored VAH/VAL            = Live berechnete Value Area
```

---

## 📊 Empfohlene Einstellungen für Bitcoin:

### CME Bitcoin Futures (BTC1!)
```
Swing Lookback............: 2-3
Big Trade Min Volume......: 50    (≈ $2-3M USD)
Value Area Percent........: 70
VWAP Width................: 2
Bubble Size...............: 15
```

### Binance/Bybit Perpetuals
```
Swing Lookback............: 3-4
Big Trade Min Volume......: 100-200
Value Area Percent........: 70
Bubble Size...............: 20
```

### Für Scalping (schnelle Swings)
```
Swing Lookback............: 1
Big Trade Min Volume......: 20-30
```

### Für Swing Trading (langsame Swings)
```
Swing Lookback............: 5-10
Big Trade Min Volume......: 100+
```

---

## ⚠️ Häufige Fehler & Lösungen

| Fehler | Grund | Lösung |
|--------|-------|--------|
| "Klassenbibliothek (.NET Framework)" fehlt | .NET Desktop nicht installiert | Visual Studio Installer → .NET Desktop-Entwicklung |
| Build-Fehler: "ATAS.Indicators nicht gefunden" | Referenzen fehlen | Alle 4 DLLs hinzufügen |
| Indikator erscheint nicht in ATAS | ATAS nicht neu gestartet | ATAS komplett schließen & neu starten |
| Keine Big Trades sichtbar | Threshold zu hoch | `BigTradeMinVolume` reduzieren (z.B. auf 10) |
| VWAP nicht sichtbar | Warten auf CHoCH | VWAP startet erst nach CHoCH! |
| "GraphicsPath nicht gefunden" | using fehlt | `using System.Drawing.Drawing2D;` hinzufügen |

---

## 🔍 Code-Anpassungen (häufig gefragt)

### Big Trade Threshold dynamisch anpassen

Im Code finden:
```csharp
public decimal BigTradeMinVolume { get; set; } = 50;
```

Ändern zu:
```csharp
public decimal BigTradeMinVolume { get; set; } = 20; // Niedrigerer Default
```

### Swing Lookback Default ändern

Im Code finden:
```csharp
public int SwingLookback { get; set; } = 2;
```

Ändern zu:
```csharp
public int SwingLookback { get; set; } = 1; // Sehr aggressive Swings
```

### Bubble-Größe anpassen

Im Code finden:
```csharp
public int BigTradeBubbleSize { get; set; } = 15;
```

Ändern zu:
```csharp
public int BigTradeBubbleSize { get; set; } = 25; // Größere Bubbles
```

---

## 📞 Support

### ATAS
- Website: https://atas.net
- Support: https://help.atas.net
- Discord: https://discord.gg/atas

### Code Issues
- Prüfe Rechtschreibung im Namespace
- Prüfe alle Referenzen
- Prüfe .NET Framework Version (muss 4.8 sein)

---

## ✅ Checkliste vor dem Start

- [ ] Visual Studio 2019/2022 installiert
- [ ] .NET Framework 4.8 installiert
- [ ] ATAS Platform installiert
- [ ] ATAS DLLs gefunden (in C:\Program Files\ATAS Platform\)
- [ ] Code-Datei `ATAS_SwingBOS_VWAP_BigTrades.cs` bereit

---

## 🚀 In 5 Minuten zum fertigen Indikator

```
1. Visual Studio öffnen                      [30 Sek]
2. Neues Projekt "Klassenbibliothek"         [1 Min]
3. Referenzen hinzufügen (4 DLLs)           [1 Min]
4. Code einfügen                            [1 Min]
5. Build (F6)                               [30 Sek]
6. In ATAS laden & neu starten              [1 Min]
────────────────────────────────────────────
TOTAL                                       [5 Min]
```

**Viel Erfolg! 🎉**
