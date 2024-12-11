namespace ElProAppServices.Test
{
    using Moq;
    using MockQueryable;
    using NUnit.Framework;

    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using ElProApp.Data.Models;
    using ElProApp.Web.Models.Building;
    using ElProApp.Services.Mapping;
    using ElProApp.Data.Models.Mappings;
    using ElProApp.Web.Models;

    [TestFixture]
    public class BuildingServiceTests : ConstructorForMock
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).Assembly);
        }

        [Test]
        public async Task AddAsync_ShouldThrowException_WhenBuildingNameIsEmpty()
        {
            var model = new BuildingInputModel
            {
                Id = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                Name = "",
                Location = "София жк Банишоара "
            }; ;
            var exception = Assert.ThrowsAsync<ArgumentException>(() => buildingService.AddAsync(model));
            Assert.AreEqual("Building name must be provided.", exception.Message);
        }

        [Test]
        public async Task AddAsync_ShouldThrowException_WhenBuildingAlreadyExists()
        {
            var model = new BuildingInputModel
            {
                Id = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                Name = "Блок 178",
                Location = "София жк Банишоара "
            };
            mockBuildingRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Building, bool>>>()))
                                   .ReturnsAsync(new Building());
            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => buildingService.AddAsync(model));
            Assert.AreEqual("A building with this name already exists!", exception.Message);
        }

        [Test]
        public async Task AddAsync_ShouldReturnId_WhenAddBuilding()
        {
            var model = new BuildingInputModel
            {
                Id = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                Name = "Блок 178",
                Location = "София жк Банишоара ",
                TeamsOnBuilding = new List<BuildingTeamMapping>()
                {
                    new BuildingTeamMapping()
                    {
                        Id = Guid.Parse("f6d8c8e9-93f4-4f7d-9e6c-b9e8c1f7e9d0"),
                        BuildingId = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                        TeamId = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77")
                    },
                    new BuildingTeamMapping()
                    {
                        Id = Guid.Parse("e5d8c7d5-92f4-4f7d-9d6c-a9e8d1f8d7c8"),
                        BuildingId = Guid.Parse("f60e7e7d-93c4-4f7c-9d6f-a8e9c1f6d7e8"),
                        TeamId = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77")
                    }
                },
                selectedTeamEntities = new List<Guid>
                {
                    Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77")
                }
            };

            var result = await buildingService.AddAsync(model);

            Assert.AreEqual(result, model.Id.ToString());
        }


        [Test]
        public async Task EditByModelAsync_ShouldThrowException_WhenModelIsNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => buildingService.EditByModelAsync(null));
            Assert.AreEqual("Building edit model cannot be null. (Parameter 'model')", exception.Message);
        }

        [Test]
        public async Task EditByModelAsync_ShouldThrowException_WhenBuildingNotFound()
        {
            var model = new BuildingEditInputModel { Id = Guid.NewGuid() };
            mockBuildingRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Building)null);

            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => buildingService.EditByModelAsync(model));
            Assert.AreEqual("Building not found.", exception.Message);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldThrowException_WhenBuildingIdIsInvalid()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(() => buildingService.SoftDeleteAsync(""));
            Assert.AreEqual("Building ID must be provided.", exception.Message);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldReturnTrue_WhenBuildingIsSuccessfullyDeleted()
        {
            var id = Guid.NewGuid().ToString();
            mockBuildingRepository.Setup(repo => repo.SoftDeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = await buildingService.SoftDeleteAsync(id);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldReturnFalse_WhenDeletionFails()
        {
            var id = Guid.NewGuid().ToString();
            mockBuildingRepository.Setup(repo => repo.SoftDeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await buildingService.SoftDeleteAsync(id);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllBuildings_WhenBuildingsExist()
        {
            var buildings = new List<Building>
            {
                building1,
                building2
            };

            var buildingTeamMappings = new List<BuildingTeamMapping>
            {
                new BuildingTeamMapping()
                {
                    Id = Guid.Parse("f6d8c8e9-93f4-4f7d-9e6c-b9e8c1f7e9d0"),
                    BuildingId = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                    TeamId = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77"),
                    Building = building1,
                    Team = team1
                },
                new BuildingTeamMapping()
                {
                    Id = Guid.Parse("e5d8c7d5-92f4-4f7d-9d6c-a9e8d1f8d7c8"),
                    BuildingId = Guid.Parse("e50f6dd1-2bd8-409f-88a3-e1d11c12cd6d"),
                    TeamId = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77"),
                    Building = building2,
                    Team = team2
                }
            };

            IQueryable<Building> queryableBuildings = buildings.BuildMock();
            mockBuildingRepository.Setup(repo => repo.GetAllAttached())
                .Returns(queryableBuildings);

            IQueryable<BuildingTeamMapping> queryableBuildingTeamMappings = buildingTeamMappings.BuildMock();
            mockBuildingTeamMappingService.Setup(repo => repo.GetAllAttached())
                .Returns(queryableBuildingTeamMappings);

            var result = await buildingService.GetAllAsync();

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetByIdAsync_ShouldThrowException_WhenBuildingNotFound()
        {
            var buildings = new List<Building>
            {
                building1
                
            };

            var buildingTeamMappings = new List<BuildingTeamMapping>
            {
                new BuildingTeamMapping()
                {
                    Id = Guid.Parse("f6d8c8e9-93f4-4f7d-9e6c-b9e8c1f7e9d0"),
                    BuildingId = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                    TeamId = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77"),
                    Building = building1,
                    Team = team1
                }
                
            };

            IQueryable<Building> queryableBuildings = buildings.BuildMock();
            mockBuildingRepository.Setup(repo => repo.GetAllAttached())
                .Returns(queryableBuildings);

            IQueryable<BuildingTeamMapping> queryableBuildingTeamMappings = buildingTeamMappings.BuildMock();
            mockBuildingTeamMappingService.Setup(repo => repo.GetAllAttached())
                .Returns(queryableBuildingTeamMappings);

            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => buildingService.GetByIdAsync(building2.Id.ToString()));
            Assert.AreEqual("Building not found or is deleted.", exception.Message);
        }

        [Test]
        public async Task EditByModelAsync_ShouldUpdateBuildingTeams_WhenTeamsAreModified()
        {
            var buildingModel = new BuildingEditInputModel()
            {

                Id = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                Name = "Блок 178",
                Location = "София жк Банишоара ",
                TeamsOnBuilding = new List<BuildingTeamMapping>()
                {
                    new BuildingTeamMapping()
                    {
                        Id = Guid.Parse("f6d8c8e9-93f4-4f7d-9e6c-b9e8c1f7e9d0"),
                        BuildingId = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                        TeamId = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77")
                    },
                    new BuildingTeamMapping()
                    {
                        Id = Guid.Parse("e5d8c7d5-92f4-4f7d-9d6c-a9e8d1f8d7c8"),
                        BuildingId = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                        TeamId = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77")
                    }
                },
                selectedTeamEntities = new List<Guid>
                {
                    Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77")
                }
            };

            var buildings = new List<Building>
            {
                building1,
                building2

            };
            var validId = building1.Id;
            IQueryable<Building> queryableBuildings = buildings.BuildMock();
            mockBuildingRepository.Setup(repo => repo.GetByIdAsync(validId))
                .ReturnsAsync(building1);

            var buildingTeamMappings = new List<BuildingTeamMapping>
            {
                new BuildingTeamMapping()
                {
                    Id = Guid.Parse("f6d8c8e9-93f4-4f7d-9e6c-b9e8c1f7e9d0"),
                    BuildingId = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                    TeamId = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77"),
                    Building = building1,
                    Team = team1
                },
                new BuildingTeamMapping()
                {
                    Id = Guid.Parse("e5d8c7d5-92f4-4f7d-9d6c-a9e8d1f8d7c8"),
                    BuildingId = Guid.Parse("e50f6dd1-2bd8-409f-88a3-e1d11c12cd6d"),
                    TeamId = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77"),
                    Building = building2,
                    Team = team2
                }

            };
            IQueryable<BuildingTeamMapping> queryableBuildingTeamMappings = buildingTeamMappings.BuildMock();
            mockBuildingTeamMappingService.Setup(repo => repo.GetAllAttached())
                .Returns(queryableBuildingTeamMappings);

            var result = await buildingService.EditByModelAsync(buildingModel);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task SoftDeleteAsync_ShouldReturnFalse_WhenBuildingDoesNotExist()
        {
            var id = Guid.NewGuid().ToString();
            mockBuildingRepository.Setup(repo => repo.SoftDeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var result = await buildingService.SoftDeleteAsync(id);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddAsync_ShouldCorrectlyAssociateTeams_WhenTeamsAreProvided()
        {
            var model = new BuildingInputModel
            {
                Id = Guid.NewGuid(),
                Name = "Building 1",
                Location = "Sofia",
                selectedTeamEntities = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            mockBuildingRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Building, bool>>>()))
                .ReturnsAsync((Building)null);

            var result = await buildingService.AddAsync(model);

            Assert.AreEqual(result, model.Id.ToString());
        }

        [Test]
        public async Task EditByModelAsync_ShouldUpdateBuildingCorrectly_WhenValidModelProvided()
        {
            var model = new BuildingEditInputModel
            {
                Id = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                Name = "Updated Building",
                Location = "Sofia Updated",
                selectedTeamEntities = new List<Guid> { Guid.NewGuid() }
            };

            var buildings = new List<Building>
            {
                building1,
                building2

            };
            
            mockBuildingRepository.Setup(repo => repo.GetByIdAsync(building1.Id))
                .ReturnsAsync(building1);

            var buildingTeamMappings = new List<BuildingTeamMapping>
            {
                new BuildingTeamMapping()
                {
                    Id = Guid.Parse("f6d8c8e9-93f4-4f7d-9e6c-b9e8c1f7e9d0"),
                    BuildingId = Guid.Parse("d67a5304-94c4-4c67-9399-321e1b823c7a"),
                    TeamId = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77"),
                    Building = building1,
                    Team = team1
                },
                new BuildingTeamMapping()
                {
                    Id = Guid.Parse("e5d8c7d5-92f4-4f7d-9d6c-a9e8d1f8d7c8"),
                    BuildingId = Guid.Parse("e50f6dd1-2bd8-409f-88a3-e1d11c12cd6d"),
                    TeamId = Guid.Parse("e13d47a8-56f3-41f7-b8a2-46df0bc6de77"),
                    Building = building2,
                    Team = team2
                }

            };
            IQueryable<BuildingTeamMapping> queryableBuildingTeamMappings = buildingTeamMappings.BuildMock();
            mockBuildingTeamMappingService.Setup(repo => repo.GetAllAttached())
                .Returns(queryableBuildingTeamMappings);

            var result = await buildingService.EditByModelAsync(model);

            var name = building1.Name;
            var location = building1.Location;

            Assert.IsTrue(result);
            Assert.AreEqual("Updated Building", name);
            Assert.AreEqual("Sofia Updated", location);
        }

        [Test]
        public async Task GetEditByIdAsync_ShouldThrowException_WhenBuildingNotFound()
        {
            var buildings = new List<Building>
            {
                building1
            };
            IQueryable<Building> queryableBuildings = buildings.BuildMock();
            mockBuildingRepository.Setup(repo => repo.GetAllAttached())
                .Returns(queryableBuildings);

            var exception = Assert.ThrowsAsync<InvalidOperationException>(() => buildingService.GetEditByIdAsync(building2.Id.ToString()));
            Assert.AreEqual("Building not found or is deleted.", exception.Message);
        }

    }
}
