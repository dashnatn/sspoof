# SSPOOF - Spoofer для SS14

Примитивная утилита для очистки HWID и данных лаунчера Space Station 14.

## Возможности

- **Проверка статуса** в реальном времени (каждую секунду)
  - HWID в реестре (hwid1, hwid2)
  - Папка данных лаунчера
- **Три режима очистки:**
  - Только HWID
  - Только данные лаунчера (с предупреждением)
  - Все вместе (с предупреждением)

## Что удаляется

- `HKEY_CURRENT_USER\Software\Space Wizards\Robust` - значения hwid1, hwid2
- `%AppData%\Space Station 14\launcher` - папка с аккаунтами и настройками (новый обход)

## Сборка (опционально (есть релиз))

```bash
dotnet publish -c Release
```
<p align="center">
  <img src="https://cdn2.cdnstep.com/Oev5POIKDurX1BddZTh5/3.thumb128.png" alt="Изображение" />
</p>
