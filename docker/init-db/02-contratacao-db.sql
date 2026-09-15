CREATE TABLE IF NOT EXISTS contratacoes (
  id uuid PRIMARY KEY,
  proposta_id uuid NOT NULL,
  data_contratacao timestamp without time zone NOT NULL
);

CREATE TABLE IF NOT EXISTS proposta_statuses (
  proposta_id uuid PRIMARY KEY,
  status text NOT NULL,
  updated_at timestamp without time zone NOT NULL
);
