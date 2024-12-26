using Microsoft.AspNetCore.Mvc;
using SimpleObject.Data;

namespace SimpleObject.Controllers
{
    // Контроллер для работы с обновлениями
    [Route("api/updates")]
    [ApiController]
    public class UpdatesController : ControllerBase
    {
        // Статическое хранилище для хранения объектов Update.
        // Используется в учебных целях; в реальных приложениях это заменяется базой данных.
        private static readonly Dictionary<Guid, Update> Updates = new();

        // Метод для обработки POST-запросов: api/updates/complex
        [HttpPost("complex")]
        public IActionResult PostComplex([FromForm] Update update)
        {
            // Проверяем корректность модели (валидность данных, обязательные поля)
            if (ModelState.IsValid && update != null)
            {
                // Кодируем текст статуса, чтобы избежать XSS-атак (внедрения HTML/JS-кода)
                update.Status = System.Net.WebUtility.HtmlEncode(update.Status);

                // Генерируем уникальный идентификатор для объекта Update
                var id = Guid.NewGuid();

                // Сохраняем объект в статическом словаре
                Updates[id] = update;

                // Возвращаем статус 201 Created с URL для получения данных объекта
                return CreatedAtAction(nameof(Status), new { id }, update);
            }

            // Если данные невалидны, возвращаем ошибку 400 Bad Request
            return BadRequest("Invalid data");
        }

        // Метод для обработки GET-запросов: api/updates/status/{id}
        [HttpGet("status/{id}")]
        public IActionResult Status(Guid id)
        {
            // Проверяем, существует ли объект с заданным идентификатором
            if (Updates.TryGetValue(id, out var update))
            {
                // Возвращаем данные объекта и статус 200 OK
                return Ok(update);
            }

            // Если объект не найден, возвращаем ошибку 404 Not Found
            return NotFound("Update not found");
        }
    }

}
