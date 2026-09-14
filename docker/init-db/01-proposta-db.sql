CREATE TABLE IF NOT EXISTS propostas (
  id uuid PRIMARY KEY,
  nomesegurado text NOT NULL,
  tiposeguro text NOT NULL,
  valorcobertura numeric NOT NULL,
  premiomensal numeric NOT NULL,
  status integer NOT NULL,
  datacriacao timestamp without time zone NOT NULL,
  dataatualizacao timestamp without time zone
);
