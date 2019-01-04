using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WhoisRH.Models;

namespace WhoisRH.Controllers
{
    public class PesquisaWhoisController : Controller
    {
        private WhoisRHDbContext db = new WhoisRHDbContext();

        public ActionResult Pesquisar(string dominio) {
            if (string.IsNullOrEmpty(dominio))
                return View();
            try {
                var pesquisa = PesquisaWhois.FromDomain(dominio);
                db.Pesquisas.Add(pesquisa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }catch(Exception e) {
                return RedirectToAction("Error", new { error = e.Message });
            }
        }

        public ActionResult Error() {
            return View();
        }

        // GET: PesquisaWhois
        public ActionResult Index()
        {
            var pesquisas = db.Pesquisas.ToList();
            pesquisas.Reverse();
            return View(pesquisas);
        }

        // GET: PesquisaWhois/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PesquisaWhois pesquisaWhois = db.Pesquisas.Find(id);
            if (pesquisaWhois == null)
            {
                return HttpNotFound();
            }
            return View(pesquisaWhois);
        }

        // GET: PesquisaWhois/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PesquisaWhois/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Dominio,DataPesquisa,Registrado,DataRegistro,UltimaAlteracao,Expiracao,NomesServidores")] PesquisaWhois pesquisaWhois)
        {
            if (ModelState.IsValid)
            {
                db.Pesquisas.Add(pesquisaWhois);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pesquisaWhois);
        }

        // GET: PesquisaWhois/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PesquisaWhois pesquisaWhois = db.Pesquisas.Find(id);
            if (pesquisaWhois == null)
            {
                return HttpNotFound();
            }
            return View(pesquisaWhois);
        }

        // POST: PesquisaWhois/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Dominio,DataPesquisa,Registrado,DataRegistro,UltimaAlteracao,Expiracao,NomesServidores")] PesquisaWhois pesquisaWhois)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pesquisaWhois).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pesquisaWhois);
        }

        // GET: PesquisaWhois/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PesquisaWhois pesquisaWhois = db.Pesquisas.Find(id);
            if (pesquisaWhois == null)
            {
                return HttpNotFound();
            }
            return View(pesquisaWhois);
        }

        // POST: PesquisaWhois/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PesquisaWhois pesquisaWhois = db.Pesquisas.Find(id);
            db.Pesquisas.Remove(pesquisaWhois);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
