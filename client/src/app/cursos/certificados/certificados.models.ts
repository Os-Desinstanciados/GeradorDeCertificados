
export interface AlunoCertificadoRequest {
    readonly nome: string;
}

export interface SolicitarCertificadosRequest {
    readonly alunos: readonly[];
}

export interface SolicitacaoCertificadoDto {
    readonly curso: string;
    readonly totalAlunos: number;
    readonly status: string;
}