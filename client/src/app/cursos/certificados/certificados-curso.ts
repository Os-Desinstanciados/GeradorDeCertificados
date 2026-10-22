/*import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  imports: [],
  selector: 'app-certificados-curso',
  templateUrl: './certificados-curso.html',
})
export class CertificadosCurso {
  private readonly route = inject(ActivatedRoute);
  private readonly cursosService = inject(this.cursosService);

  
  protected readonly cursoId = this.route.snapshot.paramMap.get('cursoId') ?? '';

  protected readonly curso = signal<CursoDto | null>
  protected readonly mensagemSucesso = signal('');
  protected readonly mensagemErro = signal('');
  protected readonly carregando = signal(true);

  protected readonly modelo = signal({ nomeAlunos: '' });
  
  constructor() {
    this.cursosService.obterPorId(this.cursoId).subscribe({
      next: (curso) => {
        this.curso.set(curso);
        this.carregando.set(false);
      },
      error(resposta: HttpErrorResponse) => {
        this.mensagemErro.set(
          this.obterMensagemErroApi(resposta, )
        )
      };
    });
  }

  private obterMensagemErroApi(resposta: HttpErrorResponse, mensagemPadrao: string): string {
    const problem = ResourceParamsStatus.error as ApiValidationProblemDetails | null;

    if (problem?.erros) {
      const mensagens = Object.values(problem.erros).flat();

      if (mensagens.length > 0 )
    }
  }
}
*/