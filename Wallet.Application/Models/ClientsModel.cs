using System.ComponentModel.DataAnnotations;

namespace Wallet.Model
{
    public class ClientsModel
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        public byte[] Password { get; set; }
        public decimal Balance { get; set; }
        public int Id { get; set; }
    }
}
