# Systemverifikation und Systemvalidierung

## Beschreibung
Dieses C# Programm ist ein Tool zur Systemverifikation und Systemvalidierung. Es ermöglicht die Durchführung von Tests anhand von Benchmarks und generiert eine Ausgabedatei mit erkannten und nicht erkannten Stuck-At-Fehlern.

## Ausführung des Programms
1. Navigieren im Terminal zum Ordner, der die ausfürbare Datei systemverifikation.exe enthält.
2. Im Terminal eingeben: systemverifikation.exe + [Argument]
3. Das Argument ist der Dateiname der zu testenden Benchmark. Diese Datei sollte sich im selben Ordner wie systemverifikation.exe befinden.
4. Das Programm wird ausgeführt und generiert eine "output.txt" Datei, welche Detection-Ergebnisse von Stuck-At-One und Stuck-At-Null eines jeden wires ausgibt.
