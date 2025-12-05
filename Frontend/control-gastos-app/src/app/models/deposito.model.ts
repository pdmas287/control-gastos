export interface Deposito {
  depositoId: number;
  fecha: Date;
  fondoMonetarioId: number;
  fondoMonetarioNombre: string;
  monto: number;
  descripcion?: string;
}

export interface DepositoCreate {
  fecha: Date;
  fondoMonetarioId: number;
  monto: number;
  descripcion?: string;
}

export interface DepositoUpdate {
  fecha: Date;
  fondoMonetarioId: number;
  monto: number;
  descripcion?: string;
}
