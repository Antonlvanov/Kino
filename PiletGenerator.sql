-- Переменные для генерации билетов
DECLARE @koht_id INT;
DECLARE @seanss_id INT;
DECLARE @klient_id INT;
DECLARE @hind DECIMAL(5,2);
DECLARE @pileti_nimi VARCHAR(100);

DECLARE kohad_cursor CURSOR FOR
SELECT koht_id, seanss_id
FROM [dbo].[kohad]
WHERE broneeritud = 1; -- Только забронированные места

OPEN kohad_cursor;

FETCH NEXT FROM kohad_cursor INTO @koht_id, @seanss_id;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Выбор случайного клиента
    SET @klient_id = ABS(CHECKSUM(NEWID())) % 52 + 1; -- Генерация случайного клиентского ID от 1 до 52

    -- Генерация стоимости билета (случайная стоимость от 7 до 12)
    SET @hind = ROUND((ABS(CHECKSUM(NEWID())) % 6 + 7), 2); -- Генерация случайной цены от 7 до 12

    -- Формирование имени билета (например, "Pilet R1-K1")
    SET @pileti_nimi = CONCAT('Pilet ', (SELECT rida FROM [dbo].[kohad] WHERE koht_id = @koht_id), '-', (SELECT koht_ridades FROM [dbo].[kohad] WHERE koht_id = @koht_id));

    -- Вставка нового билета
    INSERT INTO [dbo].[piletid] (pileti_nimi, klient_id, seanss_id, koht_id, hind)
    VALUES (
        @pileti_nimi, -- Имя билета
        @klient_id,   -- ID случайного клиента
        @seanss_id,   -- ID сеанса
        @koht_id,     -- ID места
        @hind         -- Цена билета
    );

    FETCH NEXT FROM kohad_cursor INTO @koht_id, @seanss_id;
END;

CLOSE kohad_cursor;
DEALLOCATE kohad_cursor;
