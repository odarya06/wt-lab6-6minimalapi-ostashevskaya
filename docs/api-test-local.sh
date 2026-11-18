#!/bin/bash
# Проверка CRUD локально: статусы и тела ответов.
BASE=${BASE:-http://localhost:10000}
line() { printf '%s\n' "------------------------------------------------------------"; }

line; echo "1) POST /api/products — создание товара"; line
curl -s -i -X POST "$BASE/api/products" \
  -H "Content-Type: application/json" \
  -d '{"name":"Клавиатура Logitech K120","sku":"SKU-1004","category":"Аксессуары","supplier":"ООО «ТехноСнаб»","quantity":25,"price":45.90,"description":"Проводная клавиатура для офиса","isAvailable":true,"receivedAt":"2026-11-18T00:00:00"}' \
  | sed -n '1p;/^Location/p;/^{/p'

line; echo "2) GET /api/products/7 — чтение созданного товара"; line
curl -s -w "\nСтатус: %{http_code}\n" "$BASE/api/products/7"

line; echo "3) PUT /api/products/7 — изменение товара"; line
curl -s -w "\nСтатус: %{http_code}\n" -X PUT "$BASE/api/products/7" \
  -H "Content-Type: application/json" \
  -d '{"name":"Клавиатура Logitech K120 (обновлено)","sku":"SKU-1004","category":"Аксессуары","supplier":"ООО «ТехноСнаб»","quantity":30,"price":49.90,"description":"Проводная клавиатура, обновлённая цена","isAvailable":true,"receivedAt":"2026-11-18T00:00:00"}'

line; echo "4) GET /api/products/7 — проверка изменений"; line
curl -s -w "\nСтатус: %{http_code}\n" "$BASE/api/products/7"

line; echo "5) DELETE /api/products/7 — удаление товара"; line
curl -s -o /dev/null -w "Статус: %{http_code}\n" -X DELETE "$BASE/api/products/7"

line; echo "6) GET /api/products/7 — товар удалён"; line
curl -s -o /dev/null -w "Статус: %{http_code}\n" "$BASE/api/products/7"

line; echo "7) POST /api/products — некорректные данные"; line
curl -s -w "\nСтатус: %{http_code}\n" -X POST "$BASE/api/products" \
  -H "Content-Type: application/json" \
  -d '{"name":"","sku":"SKU-9999","category":"Тест","supplier":"Тест","quantity":1,"price":10}'

line; echo "8) PUT /api/products/999 — несуществующий товар"; line
curl -s -o /dev/null -w "Статус: %{http_code}\n" -X PUT "$BASE/api/products/999" \
  -H "Content-Type: application/json" \
  -d '{"name":"Тест","sku":"SKU-9999","category":"Тест","supplier":"Тест","quantity":1,"price":10}'

line; echo "9) DELETE /api/products/999 — несуществующий товар"; line
curl -s -o /dev/null -w "Статус: %{http_code}\n" -X DELETE "$BASE/api/products/999"
