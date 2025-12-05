export interface FondoMonetario {
  fondoMonetarioId: number;
  nombre: string;
  tipoFondo: string;
  descripcion?: string;
  saldoActual: number;
  activo: boolean;
}

export interface FondoMonetarioCreate {
  nombre: string;
  tipoFondo: string;
  descripcion?: string;
  saldoActual: number;
  activo: boolean;
}

export interface FondoMonetarioUpdate {
  nombre: string;
  tipoFondo: string;
  descripcion?: string;
  activo: boolean;
}
