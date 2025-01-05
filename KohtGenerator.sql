-- Добавление мест для каждого сеанса
DECLARE @seanss_id INT;
DECLARE @saal_id INT;
DECLARE @ridade_arv INT;
DECLARE @kohad_ridades INT;
DECLARE @current_row INT;
DECLARE @current_seat INT;
DECLARE @koht_nimi VARCHAR(100);

DECLARE seanss_cursor CURSOR FOR
SELECT seanss_id, saal_id
FROM [dbo].[seansid];

OPEN seanss_cursor;

FETCH NEXT FROM seanss_cursor INTO @seanss_id, @saal_id;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Получение параметров зала
    SELECT @ridade_arv = ridade_arv, @kohad_ridades = kohad_ridades
    FROM [dbo].[saalid]
    WHERE saal_id = @saal_id;

    -- Генерация мест
    SET @current_row = 1;
    WHILE @current_row <= @ridade_arv
    BEGIN
        SET @current_seat = 1;
        WHILE @current_seat <= @kohad_ridades
        BEGIN
            -- Формирование имени места
            SET @koht_nimi = CONCAT('R', @current_row, '-K', @current_seat);

            -- Добавление места с случайным статусом бронирования
            INSERT INTO [dbo].[kohad] (koht_nimi, seanss_id, rida, koht_ridades, broneeritud)
            VALUES (
                @koht_nimi,
                @seanss_id,
                @current_row,
                @current_seat,
                CASE ABS(CHECKSUM(NEWID())) % 2
                    WHEN 0 THEN 0 -- Свободно
                    ELSE 1 -- Забронировано
                END
            );

            SET @current_seat = @current_seat + 1;
        END;
        SET @current_row = @current_row + 1;
    END;

    FETCH NEXT FROM seanss_cursor INTO @seanss_id, @saal_id;
END;

CLOSE seanss_cursor;
DEALLOCATE seanss_cursor;
