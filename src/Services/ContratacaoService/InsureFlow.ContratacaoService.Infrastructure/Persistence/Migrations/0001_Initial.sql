-- Initial migration for ContratacaoService
CREATE TABLE IF NOT EXISTS contratacoes (
  id uuid PRIMARY KEY,
  propostaid uuid NOT NULL,
  datacontratacao timestamp without time zone NOT NULL
);

CREATE TABLE IF NOT EXISTS proposta_statuses (
  propostaid uuid PRIMARY KEY,
  status text NOT NULL,
  updatedat timestamp without time zone NOT NULL
);
