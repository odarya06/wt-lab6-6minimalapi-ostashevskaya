# WT-6. REST API на ASP.NET Core Minimal API — «Складской учёт»

Лабораторная работа № 6 по дисциплине «Web-технологии».
Предметная область — **склад и логистика**, основная сущность — **Product** (товар на складе).
Модель и данные перенесены из WT-4 (ASP.NET Core MVC) и WT-5 (Razor Pages).

## Что реализовано

- проект ASP.NET Core Minimal API (`net8.0`);
- модель `Product` (9 свойств + вычисляемая стоимость партии);
- общее хранилище `ProductStore` в памяти (6 товаров, одна коллекция для всех endpoint-ов);
- полный CRUD: GET, POST, PUT, DELETE;
- параметр маршрута `{id}` и query-параметры `search`, `category`;
- корректные HTTP-статусы: 200, 201, 204, 400, 404;
- описание API через OpenAPI + Swagger UI (`/swagger`);
- `Dockerfile` с многоэтапной сборкой и поддержкой переменной `PORT`;
- публикация на Render.

## Структура проекта

```
wt-lab6-6minimalapi-ostashevskaya/
├── Models/
│   └── Product.cs
├── Data/
│   └── ProductStore.cs
├── Properties/
│   └── launchSettings.json
├── Program.cs
├── Dockerfile
├── README.md
├── .gitignore
└── wt-lab6-6minimalapi-ostashevskaya.csproj
```

## Запуск локально

```bash
dotnet restore
dotnet run
```

Приложение слушает порт `10000` (или значение переменной окружения `PORT`):
<http://localhost:10000/>.

## Таблица API

| Метод  | URL                        | Назначение                  | Успешный статус |
|--------|----------------------------|-----------------------------|-----------------|
| GET    | `/`                        | проверка работы приложения  | 200 OK          |
| GET    | `/api/info`                | информация о приложении     | 200 OK          |
| GET    | `/api/products`            | получить список товаров     | 200 OK          |
| GET    | `/api/products?search=...` | поиск по названию и артикулу| 200 OK          |
| GET    | `/api/products/{id}`       | получить товар по Id       | 200 OK / 404    |
| POST   | `/api/products`            | создать товар              | 201 Created     |
| PUT    | `/api/products/{id}`       | изменить товар             | 200 OK / 404    |
| DELETE | `/api/products/{id}`       | удалить товар              | 204 No Content  |

Некорректные данные (пустое наименование, цена ≤ 0, отрицательное количество) возвращают
`400 Bad Request` с описанием ошибки. Отсутствующий объект — `404 Not Found`.

## Примеры запросов и ответов

### GET /api/products

```http
GET http://localhost:10000/api/products
```

```json
[
  {
    "id": 1,
    "name": "Ноутбук Lenovo ThinkPad E14",
    "sku": "SKU-1001",
    "category": "Компьютеры",
    "supplier": "ООО «ТехноСнаб»",
    "quantity": 12,
    "price": 2899,
    "description": "Корпоративный ноутбук для офисных задач: 16 ГБ ОЗУ, SSD 512 ГБ, экран 14″.",
    "isAvailable": true,
    "receivedAt": "2026-11-15T00:00:00+03:00",
    "totalValue": 34788
  }
]
```

### GET /api/products/999

```http
GET http://localhost:10000/api/products/999
```

```text
404 Not Found
```

### POST /api/products

```http
POST http://localhost:10000/api/products
Content-Type: application/json
```

```json
{
  "name": "Клавиатура Logitech K120",
  "sku": "SKU-1004",
  "category": "Аксессуары",
  "supplier": "ООО «ТехноСнаб»",
  "quantity": 25,
  "price": 45.90,
  "description": "Проводная клавиатура для офиса",
  "isAvailable": true,
  "receivedAt": "2026-11-18T00:00:00"
}
```

```http
HTTP/1.1 201 Created
Location: /api/products/7
```

```json
{
  "id": 7,
  "name": "Клавиатура Logitech K120",
  "quantity": 25,
  "price": 45.90,
  "totalValue": 1147.50
}
```

### PUT /api/products/7

```http
PUT http://localhost:10000/api/products/7
Content-Type: application/json
```

```json
{
  "name": "Клавиатура Logitech K120 (обновлено)",
  "sku": "SKU-1004",
  "category": "Аксессуары",
  "supplier": "ООО «ТехноСнаб»",
  "quantity": 30,
  "price": 49.90,
  "isAvailable": true,
  "receivedAt": "2026-11-18T00:00:00"
}
```

```text
200 OK
```

### DELETE /api/products/7

```http
DELETE http://localhost:10000/api/products/7
```

```text
204 No Content
```

## OpenAPI и Swagger UI

- документ OpenAPI: <http://localhost:10000/swagger/v1/swagger.json>
- Swagger UI: <http://localhost:10000/swagger>

## Docker

```bash
docker build -t wt-lab6-6minimalapi .
docker run -p 10000:10000 wt-lab6-6minimalapi
```

Проверка после запуска контейнера:

- <http://localhost:10000/>
- <http://localhost:10000/api/products>

## Публикация

- GitHub: <https://github.com/odarya06/wt-lab6-6minimalapi-ostashevskaya>
- Render: <https://wt-lab6-6minimalapi-ostashevskaya.onrender.com>

## Тестирование

Скрипт `docs/api-test-local.sh` последовательно выполняет CRUD-цикл и печатает коды ответов:

```bash
./docs/api-test-local.sh
```

Результат локального прогона сохранён в `docs/api-test-local.txt`.

## Тестирование на Render

Тот же скрипт можно направить на опубликованный сервис через переменную `BASE`:

```bash
BASE=https://wt-lab6-6minimalapi-ostashevskaya.onrender.com ./docs/api-test-local.sh
```

Проверенные ответы публичного API: `201` при создании, `200` при чтении и обновлении,
`204` при удалении, `400` при некорректных данных, `404` для отсутствующего товара.
Свободный инстанс Render засыпает при простое, поэтому первый запрос может выполняться
до 50 секунд.
