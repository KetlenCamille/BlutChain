﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BlutChain.DAL;
using BlutChain.Models;
using BlutChain.Utils;
using Newtonsoft.Json;

namespace BlutChain.Controllers
{
    public class HemobancoController : Controller
    {
        private Context db = new Context();

        // GET: Hemobanco
        public ActionResult Index()
        {
            ViewBag.Data = DateTime.Now;
            return View(HemobancoDAO.ListarTodosHemobancos());
        }

        //[Authorize]
        public ActionResult CadastrarHemobanco()
        {
            return View((Hemobanco)TempData["Hemobanco"]);
        }

        //[Authorize]
        [HttpPost]
        public ActionResult CadastrarHemobanco(Hemobanco hemobanco)
        {
            /*[Bind(Include = "IdHemobanco, RazaoSocialHemobanco, NomeFantasiaHemobanco, CNPJHemobanco," +
            "EmailHemobanco, EnderecoHemobanco, TelefoneHemobanco")] Hemobanco hemobanco*/
            hemobanco.ehInativo = "N";

            if (ModelState.IsValid)
            {
                if (HemobancoDAO.CadastrarHemobanco(hemobanco))
                {
                    HemobancoDAO.CadastrarHemobanco(hemobanco);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Já existe um hemobanco com este CNPJ ou o CNPJ está inválido!");
                return View(hemobanco);
            }
            else
            {
                return View(hemobanco);
            }
        }

        //[Authorize]
        public ActionResult AlterarHemobanco(int? id)
        {
            if(id == null)
            {
                id = Sessao.retornarHemobanco();
            }
            return View(HemobancoDAO.BuscarHemobancoPorID(id));
        }

        //[Authorize]
        [HttpPost]
        public ActionResult AlterarHemobanco(Hemobanco hemobancoAlterado)
        {
            Hemobanco hemobancoOriginal = HemobancoDAO.BuscarHemobancoPorID(hemobancoAlterado.IdHemobanco);

            hemobancoOriginal.RazaoSocialHemobanco = hemobancoAlterado.RazaoSocialHemobanco;
            hemobancoOriginal.NomeFantasiaHemobanco = hemobancoAlterado.NomeFantasiaHemobanco;
            hemobancoOriginal.CNPJHemobanco = hemobancoAlterado.CNPJHemobanco;
            hemobancoOriginal.EmailHemobanco = hemobancoAlterado.EmailHemobanco;
            hemobancoOriginal.CEP = hemobancoAlterado.CEP;
            hemobancoOriginal.Logradouro = hemobancoAlterado.Logradouro;
            hemobancoOriginal.Numero = hemobancoAlterado.Numero;
            hemobancoOriginal.Bairro = hemobancoAlterado.Bairro;
            hemobancoOriginal.Localidade = hemobancoAlterado.Localidade;
            hemobancoOriginal.Uf = hemobancoAlterado.Uf;
            hemobancoOriginal.ehInativo = "N";
            hemobancoOriginal.EmailHemobanco = hemobancoAlterado.EmailHemobanco;

            if (ModelState.IsValid)
            {
                HemobancoDAO.AlterarHemobanco(hemobancoOriginal);
                if (Sessao.retornarUsuario() == 0)
                {
                    return RedirectToAction("PaginaInicialAdm", "Usuario");
                }
                return RedirectToAction("PaginaInicial", "Hemobanco");
            }
            else
            {
                return View(hemobancoOriginal);
            }
        }

        public ActionResult ExcluirHemobanco(int id)
        {
            Hemobanco hemobancoExcluir = HemobancoDAO.BuscarHemobancoPorID(id);
            hemobancoExcluir.ehInativo = "S";
            HemobancoDAO.AlterarHemobanco(hemobancoExcluir);
            if (Sessao.retornarUsuario() == 0)
            {
                return RedirectToAction("PaginaInicialAdm", "Usuario");
            }
            return RedirectToAction("PaginaInicial", "Hemobanco");
        }

        [HttpPost]
        public ActionResult PesquisarCEP(Hemobanco hemobanco)
        {
            try
            {
                string url = "https://viacep.com.br/ws/" + hemobanco.CEP + "/json/ ";

                WebClient client = new WebClient();
                string json = client.DownloadString(url);
                // Converter string pra UTF-8
                byte[] bytes = Encoding.Default.GetBytes(json);
                json = Encoding.UTF8.GetString(bytes);
                // Converter json para objeto
                hemobanco = JsonConvert.DeserializeObject<Hemobanco>(json);

                // Passar informação para qualquer action do controller
                TempData["Hemobanco"] = hemobanco;
            }
            catch (Exception)
            {
                TempData["Mensagem"] = "CEP Inválido!";
            }

            return RedirectToAction("CadastrarHemobanco", "Hemobanco");
        }

        public ActionResult PaginaInicial()
        {
            return View();
        }

        public ActionResult AgendamentoHemobancoHistorico()
        {
            ViewBag.Data = DateTime.Now;
            return View(AgendamentoDAO.AgendamentosHemobancoHistorico(Sessao.retornarHemobanco()));
        }

        public ActionResult AgendaHemobanco()
        {
            ViewBag.Data = DateTime.Now;
            return View(AgendamentoDAO.AgendamentosHemobancoDia(Sessao.retornarHemobanco()));
        }

        public ActionResult AgendamentosHemobancoFuturos()
        {
            ViewBag.Data = DateTime.Now;
            return View(AgendamentoDAO.AgendamentosHemobancoFuturos(Sessao.retornarHemobanco()));
        }
    }
}