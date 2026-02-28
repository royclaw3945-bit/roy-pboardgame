// Action dispatch results

export interface ValidationError {
  readonly field: string;
  readonly message: string;
}

export interface ActionSuccess {
  readonly ok: true;
  readonly message?: string;
  readonly data?: Readonly<Record<string, unknown>>;
}

export interface ActionFailure {
  readonly ok: false;
  readonly errors: readonly ValidationError[];
}

export type ActionResult = ActionSuccess | ActionFailure;

export function success(message?: string, data?: Record<string, unknown>): ActionSuccess {
  return { ok: true, message, data };
}

export function failure(errors: readonly ValidationError[]): ActionFailure {
  return { ok: false, errors };
}

export function err(field: string, message: string): ValidationError {
  return { field, message };
}
