namespace Ibatech.Domain.DTOs;

public record AlterarStatusUsuarioDto(bool Ativo);
public record RedefinirSenhaUsuarioDto(string NovaSenha);
public record AlterarMinhaSenhaDto(string SenhaAtual, string NovaSenha);
