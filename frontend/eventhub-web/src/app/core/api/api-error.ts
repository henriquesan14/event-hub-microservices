import { HttpErrorResponse } from '@angular/common/http';
import { ApiProblem } from './models';

export function apiErrorMessage(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'Não foi possível concluir a operação.';
  }

  const problem = error.error as ApiProblem | undefined;
  const validation = problem?.errors
    ? Object.values(problem.errors).flat().at(0)
    : undefined;

  return validation
    ?? problem?.detail
    ?? problem?.title
    ?? (error.status === 0
      ? 'Não foi possível conectar ao API Gateway.'
      : 'Não foi possível concluir a operação.');
}
