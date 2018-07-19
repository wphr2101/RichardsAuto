﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RichardsAuto.Data;
using RichardsAuto.Models;
using RichardsAuto.ViewModel;

namespace RichardsAuto.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ServicesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int carId)
        {
            var car = _db.Cars.FirstOrDefault(c => c.Id == carId);
            var model = new CarAndServicesViewModel
            {
                carId = car.Id,
                Make = car.Make,
                Model = car.Model,
                Style = car.Style,
                VIN = car.VIN,
                Year = car.Year,
                UserId = car.UserId,
                ServiceTypesObj = _db.ServiceTypes.ToList(),
                PastServiceObj = _db.Services.Where(s => s.CarId == carId).OrderByDescending(s => s.DateAdded)
            };
            return View(model);
        }

        // GET : Services/Create
        public IActionResult Create(int carId)
        {
            var car = _db.Cars.FirstOrDefault(c => c.Id == carId);
            var model = new CarAndServicesViewModel
            {
                carId = car.Id,
                Make = car.Make,
                Model = car.Model,
                Style = car.Style,
                VIN = car.VIN,
                Year = car.Year,
                UserId = car.UserId,
                ServiceTypesObj = _db.ServiceTypes.ToList(),
                PastServiceObj = _db.Services.Where(s => s.CarId == carId).OrderByDescending(s => s.DateAdded).Take(5)
            };
            return View(model);
        }

        // POST : Services/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarAndServicesViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.NewServiceObj.CarId = model.carId;
                model.NewServiceObj.DateAdded = DateTime.Now;
                _db.Add(model.NewServiceObj);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Create), new { carId = model.carId });
            }
            var car = _db.Cars.FirstOrDefault(c => c.Id == model.carId);
            var newModel = new CarAndServicesViewModel
            {
                carId = car.Id,
                Make = car.Make,
                Model = car.Model,
                Style = car.Style,
                VIN = car.VIN,
                Year = car.Year,                
                UserId = car.UserId,
                ServiceTypesObj = _db.ServiceTypes.ToList(),
                PastServiceObj = _db.Services.Where(s => s.CarId == model.carId).OrderByDescending(s => s.DateAdded).Take(5)
            };
            return View(newModel);
        }

        // GET : Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _db.Services.Include(s => s.Car).Include(s => s.ServiceType).SingleOrDefaultAsync(m => m.Id == id);

            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        // POST : Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Service model)
        {
            var serviceId = model.Id;
            var carId = model.CarId;
            var service = await _db.Services.SingleOrDefaultAsync(m => m.Id == serviceId);
            if (service == null)
            {
                return NotFound();
            }
            _db.Services.Remove(service);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Create), new { carId = carId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
        }
    }
}