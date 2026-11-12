# AngularApp3

Интернет-магазин: API на ASP.NET Core и фронтенд Angular в папке `shop-uni`.

API хранит пользователей, каталог, корзину и промокоды в PostgreSQL, кэширует каталог в Redis и принимает загрузку картинок через клиент Supabase Storage. В Compose рядом с API поднимается локальное хранилище с тем же контрактом. Фронтенд ходит в API по адресу из `config.json`, который контейнер подставляет при запуске.

## Стек

- ASP.NET Core 10, Entity Framework Core, PostgreSQL 16
- ASP.NET Identity и JWT
- Redis
- Supabase Storage
- Angular 21 (`shop-uni`)
- Docker Compose: API, фронтенд, PostgreSQL, Redis, локальное хранилище картинок

## Запуск в Docker

Нужны Docker и Docker Compose.

```bash
copy .env.example .env
```

В `.env` замените пароль базы, `JWT_KEY` (не короче 32 символов), почту и пароль первого администратора. Затем:

```bash
docker compose up --build
```

- Фронтенд: http://localhost:4200
- API: http://localhost:8080
- Картинки: http://localhost:54321

Данные PostgreSQL лежат в томе `postgres_data`.

При старте API применяет ожидающие миграции EF Core, затем создаёт роли, администратора и промокод `SAVE10`.

## Локальный запуск

Нужны .NET 10 SDK, Node.js, PostgreSQL и Redis.

Строка подключения, JWT и администратор для Development лежат в `AngularApp3.Server/appsettings.Development.json`. По умолчанию API ждёт PostgreSQL на `localhost:5432`, базу `angularapp3`, пользователя `postgres` без пароля, и Redis на `localhost:6379`.

```bash
dotnet run --project AngularApp3.Server --launch-profile https
```

API слушает https://localhost:7279 и http://localhost:5044.

Фронтенд:

```bash
cd shop-uni
npm ci
npm start
```

Страница открывается на http://localhost:4200. Dev-сервер проксирует `/api` на https://localhost:7279 (`shop-uni/proxy.conf.json`).

В Development первый администратор создаётся из `Admin:Email` и `Admin:Password`: `admin@localhost` / `Admin12345`.

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

## Первый администратор и промокод

При старте API создаёт роли `user` и `admin`. Регистрация всегда выдаёт роль `user`. Роль `admin` есть у учётки из `ADMIN_EMAIL` / `ADMIN_PASSWORD`. Другого администратора добавляет уже существующий через `POST /api/users/{id}/admin`. Новый токен с ролью появляется после повторного входа.

Если промокода `SAVE10` ещё нет, API создаёт его со скидкой 10%.

## API

Запросы к `/api`, кроме GET, требуют заголовок `X-XSRF-TOKEN`. Токен API кладёт в cookie `XSRF-TOKEN` на любом запросе к `/api`. Защищённые методы дополнительно ждут `Authorization: Bearer <jwt>`.

| Метод | Путь | Кто |
| --- | --- | --- |
| POST | `/api/users/register` | все |
| POST | `/api/users/login` | все |
| GET | `/api/users?page&limit&search` | admin |
| POST | `/api/users/{id}/admin` | admin |
| GET | `/api/products` | все |
| GET | `/api/products/{id}` | все |
| POST, PUT, DELETE | `/api/products` | admin |
| POST | `/api/images` | admin, файл jpeg/png/webp до 5 МБ |
| GET | `/api/cart` | user, admin |
| POST | `/api/cart/items` | user, admin |
| PATCH, DELETE | `/api/cart/items/{id}` | user, admin |
| POST | `/api/cart/apply-promo` | user, admin |
| GET, POST | `/api/promo-codes` | admin |
| DELETE | `/api/promo-codes/{id}` | admin |

Список товаров фильтруется, сортируется и разбивается на страницы в базе. Каталог кэшируется в Redis. Изменение товара сбрасывает кэш.

Спецификация OpenAPI: `/openapi/v1.json`. Swagger UI: `/swagger`. Локально это https://localhost:7279/swagger, в Compose — http://localhost:8080/swagger. В Swagger для запросов кроме GET нужен заголовок `X-XSRF-TOKEN`: его значение лежит в cookie `XSRF-TOKEN`, которую выставляет любой GET к `/api`. Для закрытых методов укажите JWT через Authorize.
