Демонстрационные ответы HTTP API (сняты при работающем сервисе и PostgreSQL).

Порядок запросов при записи этих файлов:
  1. GET  /                           → GET_root.json
  2. GET  /api/products               → GET_api_products.json
  3. GET  /api/customers              → GET_api_customers.json
  4. GET  .../customers/{id}/loyalty    → GET_api_customers_{id}_loyalty.json
  5. GET  /api/purchases              → GET_api_purchases_before_post.json (без покупок)
  6. POST /api/purchases              → тело запроса в POST_api_purchases_request.json,
                                       ответ в POST_api_purchases_response.json
  7. GET  /api/purchases              → GET_api_purchases.json
  8. GET  /api/purchases/{id}         → GET_api_purchases_{id}.json (тот же id, что в ответе POST)
  9. GET  .../purchases/{id}/receipt  → GET_api_purchases_{id}_receipt.txt (текст чека)
 10. GET  /api/products (после шага 6) → GET_api_products_after_purchase.json (остатки на складе)
 11. GET  /api/customers (после шага 6) → GET_api_customers_after_purchase.json (в т.ч. бонусы)
 12. GET  .../loyalty (несуществующий клиент) → GET_api_customers_not_found.json (404)
 13. GET  .../purchases/{несуществующий id} → GET_api_purchases_not_found.json (404)

Чеки кодов ответов см. в Swagger или заголовках при повторном запуске (успешные GET обычно 200,
POST создания покупки — 201, «не найдено» — 404).
