﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlutChain.DAL;
using BlutChain.Models;

namespace BlutChain.Controllers
{
    public class AgendamentoController : Controller
    {
        private Context context = new Context();

        // GET: Agendamento
        public ActionResult Index()
        {
            ViewBag.Data = DateTime.Now;
            return View(AgendamentoDAO.ListarAgendamentos());
        }

        public ActionResult RegistrarAgendamento()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarAgendamento([Bind(Include = "IdAgendamento,DataAgendamento,HorarioAgendamento, IdUsuario, IdHemobanco")] Agendamento agendamento)
        {
            if (ModelState.IsValid)
            {
                if (AgendamentoDAO.CadastrarAgendamento(agendamento))
                {
                    return RedirectToAction("Index", "Agendamento");
                }
                ModelState.AddModelError("", "Erro ao registrar agendamento!");
                return View(agendamento);
            }
            return View(agendamento);
        }

        public ActionResult EditarAgendamento(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agendamento agendamento = AgendamentoDAO.BuscarAgendamentoPorID(id);
            if (agendamento == null)
            {
                return HttpNotFound();
            }
            return View(agendamento);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "IdAgendamento,DataAgendamento,HorarioAgendamento, IdUsuario, IdHemobanco")] Agendamento agendamentoAlterado)
        {
            Agendamento agendamentoOriginal = AgendamentoDAO.BuscarAgendamentoPorID(agendamentoAlterado.IdAgendamento);

            agendamentoOriginal.DataAgendamento = agendamentoAlterado.DataAgendamento;
            agendamentoOriginal.HorarioAgendamento = agendamentoAlterado.HorarioAgendamento;
            agendamentoOriginal.UsuarioAgendamento = agendamentoAlterado.UsuarioAgendamento;
            agendamentoOriginal.HemobancoAgendamento = agendamentoAlterado.HemobancoAgendamento;

            if (AgendamentoDAO.EditarAgendamento(agendamentoOriginal))
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Erro ao editar agendamento!");
                return View(agendamentoOriginal);
            }
        }

    }
}