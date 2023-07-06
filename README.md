﻿## НАСТРОЙКА ОКРУЖЕНИЯ

#### 1. Настройка MySQL Workbench
- перейти по ссылке и скачать нижний файл "Windows (x86, 32-bit), MSI Installer" https://dev.mysql.com/downloads/windows/installer/
- перейти по ссылке и следовать инструкции  https://timeweb.cloud/tutorials/mysql/kak-ustanovit-mysql-na-windows
- создать БД и запустить скрипт DB.sql

#### 2. Настройка Rider
- Запустить проект в Rider
- Добавить nuget package MySql.Data
- Обновить Google.Protobuf до версии 3.21.9 или поздней

#### 3. Запустить проект 
в системе имеются три типа пользователя: администратор, специалист и обычный пользователь
- учетная запись администратора log: admin@mail.ru pass: admin
- администратор может создавать только учетные записи специалистов
- через форму регистрации может зарегистрироваться только обычный пользователь