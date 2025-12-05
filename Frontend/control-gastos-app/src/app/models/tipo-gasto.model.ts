export interface TipoGasto {
  tipoGastoId: number;
  codigo: string;
  descripcion: string;
  activo: boolean;
  usuarioId: number;
  nombreUsuario?: string;
}

export interface TipoGastoCreate {
  descripcion: string;
  activo: boolean;
}

export interface TipoGastoUpdate {
  descripcion: string;
  activo: boolean;
}
