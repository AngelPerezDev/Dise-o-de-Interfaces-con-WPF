-- Table: peliculas
DROP TABLE IF EXISTS peliculas;

CREATE TABLE peliculas (
    idPelicula   INTEGER PRIMARY KEY AUTOINCREMENT,
    titulo       TEXT,
    cartel       TEXT,
    año          INTEGER,
    genero       TEXT,
    calificacion TEXT
);


-- Table: salas
CREATE TABLE IF NOT EXISTS salas (
    idSala     INTEGER PRIMARY KEY AUTOINCREMENT,
    capacidad  INTEGER,
    disponible BOOLEAN DEFAULT (true) 
);


-- Table: sesiones
CREATE TABLE IF NOT EXISTS sesiones (
    idSesion INTEGER PRIMARY KEY AUTOINCREMENT,
    pelicula TEXT,
    sala     INTEGER REFERENCES salas (idSala),
    hora     TEXT
);


-- Table: ventas
CREATE TABLE IF NOT EXISTS ventas (
    idVenta  INTEGER PRIMARY KEY AUTOINCREMENT,
    sesion   INTEGER REFERENCES sesiones (idSesion),
    cantidad INTEGER,
    pago     TEXT
);
