using DataAccess.Interface;
using Microsoft.Extensions.DependencyInjection;
using DTO.Label;
using FluentAssertions;

namespace Test_Unit {
    [TestFixture]
    public class DAL_LabelRepository {
        private ILabelRepository _labelRepository;

        [SetUp]
        public async Task Setup() {
            var serviceProvider = TestDependencySetup.BuildServiceProvider();
            _labelRepository = serviceProvider.GetRequiredService<ILabelRepository>();
            await TestHelpers.ResetDatabaseWithChangeSql(serviceProvider);
        }


        [Test]
        public async Task CreateLabel_ShouldPass() {
            //Arrange
            var label = new DTOLabel {
                CustomerId = "OUH",
                ContextId = null,
                Name = "NewLabel",
                Description = "This is a new label",
                Value = null
            };
            //Act
            await _labelRepository.CreateNewLabelAsync(label);
            var listOfLabels = await _labelRepository.GetAllLabelsByCustomerId("OUH");
            //Assert
            listOfLabels.Should().ContainSingle(l => l.Name == "NewLabel" && l.Description == "This is a new label");
        }

        [Test]
        public async Task DeleteLabel_ShouldPass() {
            //Arrange
            var label = new DTOLabel {
                CustomerId = "ABENA",
                ContextId = null,
                Name = "ToDelete",
                Description = "Label to be deleted",
                Value = null
            };
            //Act
            await _labelRepository.CreateNewLabelAsync(label);
            var listOfLabels = await _labelRepository.GetAllLabelsByCustomerId("ABENA");
            var labelToDelete = listOfLabels.Find(l => l.Name == "ToDelete");
            //Assert
            await _labelRepository.DeleteLabelAsync(labelToDelete!);
            var listOfLabelsAfterDelete = await _labelRepository.GetAllLabelsByCustomerId("ABENA");
            listOfLabelsAfterDelete.Should().NotContain(l => l.Name == "ToDelete");
        }

        [Test]
        public async Task UpdateLabel_ShouldPass() {
            //Arrange
            var label = new DTOLabel {
                CustomerId = "OUH",
                ContextId = null,
                Name = "ToUpdate",
                Description = "Label to be updated",
                Value = null
            };
            await _labelRepository.CreateNewLabelAsync(label);
            var listOfLabels = await _labelRepository.GetAllLabelsByCustomerId("OUH");
            ////Act
            var createdLabel = listOfLabels.Find(l => l.Name == "ToUpdate");
            createdLabel!.Description = "Label has been updated";
            await _labelRepository.UpdateLabelAsync(createdLabel!);
            var updatedListOfLabels = await _labelRepository.GetAllLabelsByCustomerId("OUH");
            ////Assert
            updatedListOfLabels.Should().ContainSingle(l => l.Name == "ToUpdate" && l.Description == "Label has been updated");
        }

        [Test]
        public async Task GetAllLabelsByCustomerId_ShouldPass() {
            //Arrange
            var label1 = new DTOLabel {
                CustomerId = "OUH",
                ContextId = null,
                Name = "Label1",
                Description = "First label",
                Value = null
            };
            var label2 = new DTOLabel {
                CustomerId = "OUH",
                ContextId = null,
                Name = "Label2",
                Description = "Second label",
                Value = null
            };
            await _labelRepository.CreateNewLabelAsync(label1);
            await _labelRepository.CreateNewLabelAsync(label2);
            //Act
            var listOfLabels = await _labelRepository.GetAllLabelsByCustomerId("OUH");
            //Assert
            listOfLabels.Should().Contain(l => l.Name == "Label1");
            listOfLabels.Should().Contain(l => l.Name == "Label2");
        }

        [Test]
        public async Task GetAllLabelsByContextID_ShouldPass() {
            //Arrange
            var label1 = new DTOLabel {
                CustomerId = "OUH",
                ContextId = 1,
                Name = "ContextLabel1",
                Description = "First context label",
                Value = null
            };
            var label2 = new DTOLabel {
                CustomerId = "OUH",
                ContextId = 1,
                Name = "ContextLabel2",
                Description = "Second context label",
                Value = null
            };
            await _labelRepository.CreateNewLabelAsync(label1);
            await _labelRepository.CreateNewLabelAsync(label2);
            //Act
            var listOfLabels = await _labelRepository.GetAllLabelsByContextId(1);
            //Assert
            listOfLabels.Should().Contain(l => l.Name == "ContextLabel1");
            listOfLabels.Should().Contain(l => l.Name == "ContextLabel2");
        }
    }
}
