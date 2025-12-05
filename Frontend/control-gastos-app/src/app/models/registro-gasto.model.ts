export interface RegistroGasto {
  registroGastoId: number;
  fecha: Date;
  fondoMonetarioId: number;
  fondoMonetarioNombre: string;
  nombreComercio: string;
  tipoDocumento: string;
  observaciones?: string;
  montoTotal: number;
  detalles: RegistroGastoDetalle[];
}

export interface RegistroGastoDetalle {
  registroGastoDetalleId: number;
  tipoGastoId: number;
  tipoGastoDescripcion: string;
  monto: number;
  descripcion?: string;
}

export interface RegistroGastoCreate {
  fecha: Date;
  fondoMonetarioId: number;
  nombreComercio: string;
  tipoDocumento: string;
  observaciones?: string;
  detalles: RegistroGastoDetalleCreate[];
}

export interface RegistroGastoDetalleCreate {
  tipoGastoId: number;
  monto: number;
  descripcion?: string;
}

export interface ValidacionPresupuesto {
  haySobregiro: boolean;
  sobregiros: SobregiroDetalle[];
}

export interface SobregiroDetalle {
  tipoGastoId: number;
  tipoGastoDescripcion: string;
  montoPresupuestado: number;
  montoEjecutado: number;
  montoSobregiro: number;
}
