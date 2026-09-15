-- Initial migration for PropostaService
CREATE TABLE IF NOT EXISTS propostas (
  id uuid PRIMARY KEY,
  nome_segurado text NOT NULL,
  tipo_seguro text NOT NULL,
  valor_cobertura numeric NOT NULL,
  premio_mensal numeric NOT NULL,
  status integer NOT NULL,
  data_criacao timestamp without time zone NOT NULL,
  data_atualizacao timestamp without time zone
);
