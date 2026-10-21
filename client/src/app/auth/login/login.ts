import { afterNextRender, Component, ElementRef, inject, signal, viewChild } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { LucideArrowRight } from '@lucide/angular';
import { AuthShell } from '../shell/auth-shell';
import { ApiValidationProblemDetails, CadastroConcluidoNavigationState, LoginRequest, TokenDto } from '../auth.models';
import { form, required, email, minLength, pattern, FormField } from '@angular/forms/signals';
import { AuthService } from '../auth.service';
import { Observer } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

interface LoginFormModel {
  readonly email: string;
  readonly senha: string;
}
@Component({
  imports: [AuthShell, LucideArrowRight, FormField, RouterLink],
  selector: 'app-login',
  templateUrl: './login.html',
})
export class Login {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly tituloPagina = viewChild<ElementRef<HTMLHeadElement>>('tituloPagina');

  protected readonly modeloLogin = signal<LoginFormModel>({ email: '', senha: '' });
  protected readonly formularioLogin = form(this.modeloLogin, (modelo) => {
    required(modelo.email, { message: 'Informe seu email.' });
    email(modelo.email, { message: 'Informe um email válido.' });
    required(modelo.senha, { message: 'Informe sua senha.' });
    minLength(modelo.senha, 8, { message: 'A senha precisa ter pelo menos 8 caracteres.' });
  });

  protected readonly mensagemSucesso = signal('');
  protected readonly mensagemErro = signal('');

  constructor() {
    afterNextRender(() => this.tituloPagina()?.nativeElement.focus());

    const estadoNavegacao = this.router.currentNavigation()?.extras.state as
      CadastroConcluidoNavigationState | undefined;

    if (estadoNavegacao?.emailCadastrado) {
      this.modeloLogin.update((original) => ({
        email: estadoNavegacao?.emailCadastrado ?? '',
        senha: original.senha,
      }));

      this.mensagemSucesso.set(
        'Conta criada com sucesso. Entre com suas credenciais para continuar.',
      );
    }
  }

  protected enviarFormulario(event: SubmitEvent): void {
    event.preventDefault();

    if (this.formularioLogin().invalid()) {
      this.formularioLogin().markAsTouched();
      return;
    }

    const modelo = this.modeloLogin();

    const requisicao: LoginRequest = {
      email: modelo.email,
      senha: modelo.senha,
    };

    this.mensagemErro.set('');

    const observer: Partial<Observer<TokenDto>> = {
      next: () => void this.router.navigateByUrl('/'),
      error: (response: HttpErrorResponse) => {
        const mensagem = response.status === 401 
          ? 'Email ou senha incorretos. Confirme suas credenciais e tente novamente.'
          : this.ObterMensagemErroApi(response);

        this.mensagemErro.set('');
      },
    };

    this.authService.login(requisicao).subscribe(observer);
  }

  private ObterMensagemErroApi(response: HttpErrorResponse): string{
    const problem = response.error as ApiValidationProblemDetails | null;

    return problem?.detail ?? 'Não foi possível conectar à API. Tem novamente em instantes.';
  }
}
