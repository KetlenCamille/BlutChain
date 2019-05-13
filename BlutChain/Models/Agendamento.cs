﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlutChain.Models
{
    public class Agendamento
    {
        public int IdAgendamento { get; set; }

        public DateTime DataAgendamento { get; set; }

        public DateTime HorarioAgendamento { get; set; }

        public Usuario UsuarioAgendamento { get; set; }

        public Hemobanco HemobancoAgendamento { get; set; }
    }
}