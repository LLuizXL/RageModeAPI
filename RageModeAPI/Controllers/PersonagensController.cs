using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RageModeAPI.Data;
using RageModeAPI.Models;

namespace RageModeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PersonagensController : ControllerBase
    {
        private readonly RageModeApiContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PersonagensController(RageModeApiContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/Personagens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Personagem>>> GetPersonagens([FromQuery] string? jogoNome)
        {
            var query = _context.Personagens
                .Include(p => p.TipoPersonagem)
                .Include(p => p.Jogo)
                .AsQueryable();

            if (!string.IsNullOrEmpty(jogoNome))
                query = query.Where(p => p.Jogo.JogoNome == jogoNome);

            var personagens = await query.ToListAsync();
            return Ok(personagens);
        }

        // GET: api/Personagens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Personagem>> GetPersonagem(Guid id)
        {
            var personagem = await _context.Personagens.FindAsync(id);

            if (personagem == null)
            {
                return NotFound();
            }

            return personagem;
        }

        // PUT: api/Personagens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersonagem(Guid id, Personagem personagem)
        {
            if (id != personagem.PersonagemId)
            {
                return BadRequest();
            }

            _context.Entry(personagem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonagemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Personagens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<Personagem>> PostPersonagem(Personagem personagem)
        {
            _context.Personagens.Add(personagem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPersonagem", new { id = personagem.PersonagemId }, personagem);


        }

        // DELETE: api/Personagens/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersonagem(Guid id)
        {
            var personagem = await _context.Personagens.FindAsync(id);
            if (personagem == null)
            {
                return NotFound();
            }

            _context.Personagens.Remove(personagem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonagemExists(Guid id)
        {
            return _context.Personagens.Any(e => e.PersonagemId == id);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("UploadCharacterPicture")]
        public async Task<IActionResult> UploadCharacterPicture(IFormFile file, Guid PersonagemId)
        {
            // Verifica se o arquivo é nulo ou vazio
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo não pode ser nulo ou vazio.");

            // Verifica se o personagem existe
            var personagem = await _context.Personagens.FindAsync(PersonagemId);
            if (personagem == null)
                return NotFound("Usuário não encontrado.");

            // Verifica se o arquivo é uma imagem
            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("O arquivo deve ser uma imagem.");

            // Define o caminho para salvar a imagem na pasta Resources/Profile
            var personagemFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "Personagens");
            if (!Directory.Exists(personagemFolder))
                Directory.CreateDirectory(personagemFolder);

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (Array.IndexOf(allowedExtensions, fileExtension) < 0)
                return BadRequest("Formato de arquivo não suportado. Use .jpg, .jpeg, .png ou .gif.");

            var fileName = $"{personagem.PersonagemId}{fileExtension}";
            var filePath = Path.Combine(personagemFolder, fileName);


            // Verifica se o arquivo já existe e o remove
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Salva o arquivo no disco
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retorna o caminho relativo da imagem
            var relativePath = Path.Combine("Resources", "Personagens", fileName).Replace("\\", "/");

            // Atualiza o Campo Banner do personagem
            personagem.Personagemimage = fileName;
            _context.Entry(personagem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Retorna o caminho da imagem
            return Ok(new { FilePath = relativePath });
        }


        // [HttpGET] : Buscar a imagem de personagem e retornar como Base64
        [HttpGet("GetCharacterPicture")]
        public async Task<IActionResult> GetCharacterPicture(Guid PersonagemId)
        {
            // Verifica se o personagem existe
            var personagem = await _context.Personagens.FindAsync(PersonagemId);
            if (personagem == null)
                return NotFound("personagem não encontrado.");

            // Caminho da imagem na pasta Resources/personagems
            var personagemFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Resources", "Personagens");
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            // Procura a imagem do personagem com base no ID
            string? personagemImagePath = null;
            foreach (var extension in allowedExtensions)
            {
                var potentialPath = Path.Combine(personagemFolder, $"{personagem.PersonagemId}{extension}");
                if (System.IO.File.Exists(potentialPath))
                {
                    personagemImagePath = potentialPath;
                    break;
                }
            }
            // Se a imagem não for encontrada
            if (personagemImagePath == null)
                return NotFound("Imagem de perfil não encontrada.");

            // Lê o arquivo como um array de bytes
            var imageBytes = await System.IO.File.ReadAllBytesAsync(personagemImagePath);

            // Converte os bytes para Base64
            var base64Image = Convert.ToBase64String(imageBytes);

            // Retorna a imagem em Base64
            return Ok(new { Base64Image = $"data:image/{Path.GetExtension(personagemImagePath).TrimStart('.')};base64,{base64Image}" });
        }
    }
}
