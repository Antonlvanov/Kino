-- Вставка данных в таблицу seansid
DECLARE @start_date DATE = '2025-01-06';
DECLARE @end_date DATE = '2025-01-12';

-- Переменные для генерации сеансов
DECLARE @current_date DATE;
DECLARE @film_count INT = 8; -- Количество фильмов в таблице filmid
DECLARE @hall_count INT = 5; -- Количество залов
DECLARE @sessions_per_day INT = 7; -- Среднее количество сеансов в день
DECLARE @duration INT;

SET @current_date = @start_date;

WHILE @current_date <= @end_date
BEGIN
    DECLARE @session_counter INT = 1;

    WHILE @session_counter <= @sessions_per_day
    BEGIN
        -- Выбор случайного фильма и зала
        DECLARE @film_id INT = ABS(CHECKSUM(NEWID())) % @film_count + 1;
        DECLARE @hall_id INT = ABS(CHECKSUM(NEWID())) % @hall_count + 1;

        -- Извлечение продолжительности фильма из таблицы filmid
        SELECT @duration = kestus FROM [dbo].[filmid] WHERE film_id = @film_id;

        -- Генерация случайного времени начала сеанса между 09:00 и 22:00
        DECLARE @random_minutes INT = ABS(CHECKSUM(NEWID())) % ((22 - 9) * 60); -- Случайное количество минут между 09:00 и 22:00
        DECLARE @base_time DATETIME = CAST(@current_date AS DATETIME) + CAST('09:00:00' AS DATETIME);
        DECLARE @start_time DATETIME = DATEADD(MINUTE, @random_minutes, @base_time);
        DECLARE @end_time DATETIME = DATEADD(MINUTE, @duration, @start_time);

        -- Проверка значений
        IF @duration IS NOT NULL AND @start_time IS NOT NULL AND @end_time IS NOT NULL AND @end_time <= DATEADD(SECOND, -1, CAST(@current_date AS DATETIME) + CAST('22:00:00' AS DATETIME))
        BEGIN
            -- Добавление сеанса
            INSERT INTO [dbo].[seansid] (seansi_nimi, film_id, saal_id, kuupaev, alus_aeg, lopp_aeg)
            VALUES (
                CONCAT('Seanss ', @film_id, '-', @hall_id, '-', FORMAT(@current_date, 'yyyyMMdd')), 
                @film_id, 
                @hall_id, 
                @current_date, 
                @start_time, 
                @end_time
            );
        END;

        SET @session_counter = @session_counter + 1;
    END;

    SET @current_date = DATEADD(DAY, 1, @current_date);
END;
