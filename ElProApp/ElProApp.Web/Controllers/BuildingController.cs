﻿namespace ElProApp.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using ElProApp.Services.Data.Interfaces;
    using Models.Building;
    using static Common.ApplicationConstants;

    /// <summary>
    /// Controller for managing building entries and operations.
    /// </summary>
    [Authorize]
    public class BuildingController(IBuildingService buildingService, IBuildingTeamMappingService buildingTeamMappingService) : Controller
    {
        /// <summary>
        /// Displays a list of all Buildings.
        /// </summary>
        /// <returns>A view with the list of all Buildings.</returns>
        [HttpGet]
        public async Task<IActionResult> All()
        {
            if (User.IsInRole(AdminRoleName) || User.IsInRole(OfficeManagerRoleName) || User.IsInRole(TechnicianRoleName))
                return RedirectToAction("AllBuildings", "Admin", new { area = "admin" });

            return View(await buildingService.GetAllAsync());
        }

        /// <summary>
        /// Displays the form for adding a new building.
        /// Accessible only by administrators.
        /// </summary>
        /// <returns>A view for adding a building.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician")]
        [HttpGet]
        public async Task<IActionResult> Add() => View(new BuildingInputModel()
        {
            TeamsOnBuilding = await buildingTeamMappingService.GetAllAttachedAsync()
        });

        /// <summary>
        /// Processes the request to add a new building.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="model">The building model containing the new building details.</param>
        /// <returns>Redirects to the building details view or stays on the page if there's an error.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician")]
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

        /// <summary>
        /// Displays the details of a building by its ID.
        /// </summary>
        /// <param name="id">The ID of the building.</param>
        /// <returns>A view with the details of the specified building.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician , Worker")]
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

        /// <summary>
        /// Displays the form for editing an existing building.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="id">The ID of the building to edit.</param>
        /// <returns>A view for editing the building details.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician")]
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

        /// <summary>
        /// Processes the request to update the building details.
        /// Accessible only by administrators.
        /// </summary>
        /// <param name="model">The model containing updated building data.</param>
        /// <returns>Redirects to the building details view or stays on the page if there's an error.</returns>
        [Authorize(Roles = "Admin , OfficeManager , Technician")]
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

        [Authorize(Roles = "Admin , OfficeManager")]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var idCheck = string.IsNullOrWhiteSpace(id);
            if (idCheck) throw new BadHttpRequestException("Building ID cannot be empty.");

            var employee = await buildingService.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound("Building not found.");
            }

            bool isDeleted = await buildingService.SoftDeleteAsync(id);
            if (isDeleted)
            {
                return RedirectToAction(nameof(All));
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
