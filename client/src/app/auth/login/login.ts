import { afterNextRender, Component, ElementRef, inject, signal, viewChild } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { LucideArrowRight } from '@lucide/angular';
import { AuthShell } from '../shell/auth-shell';
import { CadastroConcluidoNavigationState, LoginRequest } from '../auth.models';
import { form, required, email, minLength, pattern, FormField } from '@angular/forms/signals';
import { AuthService } from '../auth.service';

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

  protected readonly modeloFormulario = signal<LoginFormModel>({ email: '', senha: '' });

  protected readonly formularioLogin = form(this.modeloFormulario, (modelo) => {
    required(modelo.email, { message: 'Informe seu email.' });
    email(modelo.email, { message: 'Informe um email válido.' });
    required(modelo.senha, { message: 'Informe sua senha.' });
    minLength(modelo.senha, 8, { message: 'A senha precisa ter pelo menos 8 caracteres.' });
  });

  constructor() {
    afterNextRender(() => this.tituloPagina()?.nativeElement.focus());

    const estadoNavegacao = this.router.currentNavigation()?.extras.state as
      CadastroConcluidoNavigationState | undefined;

    if (estadoNavegacao?.emailCadastrado) {
      this.modeloFormulario.update((original) => ({
        email: estadoNavegacao?.emailCadastrado ?? '',
        senha: original.senha,
      }));
    }
  }

  protected enviarFormulario(event: SubmitEvent): void {
    event.preventDefault();

    if (this.formularioLogin().invalid()) {
      this.formularioLogin().markAsTouched();
      return;
    }

    const requisicao: LoginRequest = {
      email: this.modeloFormulario().email,
      senha: this.modeloFormulario().senha,
    };

    this.authService.login(requisicao).subscribe(()=> {
      void this.router.navigateByUrl('/');
    });
  }
}
