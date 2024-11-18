﻿namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;    

    using ElProApp.Services.Data.Interfaces;
    using Models.Building;

    public class BuildingController(IBuildingService _buildingService, IBuildingTeamMappingService _buildingTeamMappingService) : Controller
    {
        private readonly IBuildingService buildingService = _buildingService;
        private readonly IBuildingTeamMappingService buildingTeamMappingService = _buildingTeamMappingService;

        [HttpGet]
        public async Task<IActionResult> All() => View(await buildingService.GetAllAsync());

        [HttpGet]
        public async Task<IActionResult> Add() => View(new BuildingInputModel()
        {
            TeamsOnBuilding = await buildingTeamMappingService.GetAllAttachedAsync()
        });

        [HttpPost]
        public async Task<IActionResult> Add(BuildingInputModel model)
        {
            if (!ModelState.IsValid)
            {
                model.TeamsOnBuilding = await buildingTeamMappingService.GetAllAttachedAsync();
                return View(model);
            }
            try
            {
                string buildingId = await buildingService.AddAsync(model);
                return RedirectToAction(nameof(Details), new { id = buildingId });
            }
            catch
            {
                return RedirectToAction(nameof(All));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var model = await buildingService.GetByIdAsync(id);
                
                return View(model);
            }
            catch
            {
                return RedirectToAction(nameof(All));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var model = await buildingService.GetEditByIdAsync(id);               

                return View(model);
            }
            catch
            {
                return RedirectToAction(nameof(All));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BuildingEditInputModel model)
        {
            if (!ModelState.IsValid)
            {
                model.TeamsOnBuilding = await buildingTeamMappingService.GetAllAttachedAsync();
                return View(model);
            }

            bool isEdited = await buildingService.EditByModelAsync(model);
            if (!isEdited)
            {
                model.TeamsOnBuilding = await buildingTeamMappingService.GetAllAttachedAsync();
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
    }
}
