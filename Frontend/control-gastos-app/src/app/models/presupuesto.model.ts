export interface Presupuesto {
  presupuestoId: number;
  tipoGastoId: number;
  tipoGastoDescripcion: string;
  mes: number;
  anio: number;
  montoPresupuestado: number;
}

export interface PresupuestoCreate {
  tipoGastoId: number;
  mes: number;
  anio: number;
  montoPresupuestado: number;
}

export interface PresupuestoUpdate {
  montoPresupuestado: number;
}

export interface PresupuestosPorMes {
  mes: number;
  anio: number;
  items: PresupuestoItem[];
}

export interface PresupuestoItem {
  presupuestoId: number;
  tipoGastoId: number;
  tipoGastoDescripcion: string;
  montoPresupuestado: number;
}
