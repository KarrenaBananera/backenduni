# AngularApp3

Интернет-магазин: API на ASP.NET Core и Angular-фронтенд в папке `shop-uni`.

Данные лежат в PostgreSQL, каталог кэшируется в Redis. При старте API само применяет миграции, создаёт роли, первого администратора и промокод `SAVE10`.

## Локально

Нужны .NET 10 SDK, Node.js, PostgreSQL и Docker (для Redis).

PostgreSQL: `localhost:5432`, база `angularapp3`, пользователь `postgres` без пароля. Эти значения лежат в `AngularApp3.Server/appsettings.Development.json`.

```bash
docker compose up -d redis
dotnet run --project AngularApp3.Server --launch-profile https
```

API: https://localhost:7279 и http://localhost:5044.

В другом терминале:

```bash
cd shop-uni
npm ci
npm start
```

Сайт: http://localhost:4200. Запросы к `/api` уходят на https://localhost:7279.

Администратор: `admin@localhost` / `Admin12345`.

Локальная база берётся из `appsettings.Development.json`. Пароль базы и ключ Storage в репозиторий не входят: на Koyeb их задают переменные `ConnectionStrings__DefaultConnection` и `Supabase__ServiceRoleKey`.

## Docker

Нужны Docker и Docker Compose.

```bash
copy .env.example .env
docker compose up --build
```

В `.env` замените пароль базы, `JWT_KEY` (не короче 32 символов), почту и пароль администратора. Остальные поля можно оставить как в примере.

- Сайт: http://localhost:4200
- API: http://localhost:8080

## Переменные окружения

Файл `.env` в git не попадает. Пример — `.env.example`.

| Переменная | Назначение |
| --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Окружение API. В примере Compose стоит `Production`. |
| `ASPNETCORE_URLS` | Адрес, на котором слушает API внутри контейнера. |
| `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB` | Учётная запись и имя базы PostgreSQL. |
| `JWT_ISSUER`, `JWT_AUDIENCE`, `JWT_KEY` | Проверка JWT. `JWT_KEY` не короче 32 символов. |
| `ADMIN_EMAIL`, `ADMIN_PASSWORD` | Первый администратор. Если одно из полей пустое, учётка не создаётся. Повторный запуск пароль не меняет. |
| `CORS_ALLOWED_ORIGINS` | Разрешённые origin через запятую. Нужны, если фронтенд открыт с другого адреса, чем API. |
| `ANTIFORGERY_SAMESITE` | `Lax` для одного сайта, `None` для запросов с другого origin по HTTPS. |
| `SUPABASE_URL` | Базовый адрес Storage, который без изменений получает официальный клиент. В примере это `http://storage:5000/storage/v1`. Для облака укажите `https://<project>.supabase.co/storage/v1`. |
| `SUPABASE_PUBLIC_URL` | Тот же базовый адрес, но доступный браузеру. Им заменяется `SUPABASE_URL` в готовой ссылке. В примере это `http://localhost:54321/storage/v1`. Для облака оставьте пустым. |
| `SUPABASE_OBJECT_PREFIX`, `SUPABASE_PUBLIC_PREFIX` | Пути локального хранилища для загрузки и для публичной выдачи файла. Облачному Supabase не нужны. |
| `SUPABASE_SERVICE_ROLE_KEY`, `SUPABASE_BUCKET` | Ключ и бакет. Если URL, ключ или бакет пустые, API сохраняет файл у себя и отдаёт его по `/api/images/{имя}`. |
| `FRONTEND_API_URL` | Адрес API для браузера. По умолчанию `/api`. |
| `FRONTEND_API_PROXY` | Куда nginx фронтенда проксирует `/api`. По умолчанию `http://app:8080`. |

Для отдельного деплоя фронтенда те же значения задаются контейнеру как `API_URL` и `API_PROXY`. Если API на другом домене, укажите полный `API_URL`, например `https://api.example.com/api`, и добавьте origin фронтенда в `CORS_ALLOWED_ORIGINS`. При `API_URL=/api` браузер и API остаются на одном origin, и cookie антифорджери работает с `Lax`.

## Swagger

Локально: https://localhost:7279/swagger. В Docker: http://localhost:8080/swagger.

Для любого запроса кроме GET нужен заголовок `X-XSRF-TOKEN`. В Development вызовите любой GET к `/api` и скопируйте этот заголовок из ответа. Для закрытых методов нажмите Authorize и вставьте JWT из ответа логина.
