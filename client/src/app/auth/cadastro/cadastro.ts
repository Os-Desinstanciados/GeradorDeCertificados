import { afterNextRender, Component, ElementRef, model, signal, viewChild } from '@angular/core';
import { AuthShell } from '../shell/auth-shell';
import { LucideArrowRight } from '@lucide/angular';
import { RouterLink } from '@angular/router';
import { email, form, FormField, minLength, pattern, required } from '@angular/forms/signals';
import { CadastroRequest } from '../auth.models';

interface CadastroFormModel {
  readonly email: string;
  readonly senha: string;
}

@Component({
  imports: [AuthShell, LucideArrowRight, FormField, RouterLink, AuthShell],
  selector: 'app-cadastro',
  templateUrl: './cadastro.html',
})
export class Cadastro {
  private readonly tituloPagina = viewChild<ElementRef<HTMLHRElement>>('tituloPagina');

  protected readonly modeloFormulario = signal<CadastroFormModel>({ email: '', senha: '' });

  protected readonly formularioCadastro = form(this.modeloFormulario, (modelo) => {
    required(modelo.email, { message: 'Informe seu email.' });
    email(modelo.email, { message: 'Informe um email válido.' });

    required(modelo.senha, { message: 'Informe sua senha.' });
    minLength(modelo.senha, 8, { message: 'A senha precisa ter pelo menos 8 caracteres.' });
    pattern(modelo.senha, /(?=.*\d)(?=.*[^a-zA-Z0-9]).+/, {
      message: 'Inclua pelo menos um número e um caractere especial.',
    });
  });
  
  constructor() {
    afterNextRender(() => this.tituloPagina()?.nativeElement.focus());
  }

  enviarFormulario(event: SubmitEvent) {
    event.preventDefault();

    const request: CadastroRequest = {
      email: this.modeloFormulario().email,
      senha: this.modeloFormulario().senha,
    };
  }
}
