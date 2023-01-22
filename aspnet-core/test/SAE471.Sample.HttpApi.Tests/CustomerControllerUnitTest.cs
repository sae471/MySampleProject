
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SAE471.Sample.Application.Contracts;
using SAE471.Sample.HttpApi.Host.Controllers;
using SAE471;

namespace SAE471.Sample.HttpApi.Tests;

public class CustomerControllerUnitTest
{
    public Guid ExistedCutomerId { get { return new Guid("052e813b-1a4a-44cd-7f3b-08dafc7a172f"); } }

    #region Get By Id

    [Fact]
    public async void GetCustomerById_Return_OkResult()
    {
        //Arrange
        var controller = new CustomerController();
        var customerId = ExistedCutomerId;


        //Act
        var data = await controller.Get(customerId);

        //Assert
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async void GetCustomerById_Return_NotFoundResult()
    {
        //Arrange
        var controller = new CustomerController();
        var customerId = Guid.NewGuid();


        //Act
        var data = await controller.Get(customerId);

        //Assert
        Assert.IsType<NotFoundResult>(data);
    }

    [Fact]
    public async void GetCustomerById_Return_BadRequestResult()
    {
        //Arrange
        var controller = new CustomerController();
        var customerId = Guid.Empty;

        //Act
        var data = await controller.Get(customerId);

        //Assert
        Assert.IsType<BadRequestResult>(data);
    }

    [Fact]
    public async void GetCustomerById_MatchResult()
    {
        //Arrange
        var controller = new CustomerController();
        var customerId = ExistedCutomerId;


        //Act
        var data = await controller.Get(customerId);

        //Assert
        Assert.IsType<OkObjectResult>(data);

        var okResult = data.Should().BeOfType<OkObjectResult>().Subject;
        var customer = okResult.Value.Should().BeAssignableTo<CustomerDTO>().Subject;

        Assert.Equal("Abbas", customer.FirstName);
        Assert.Equal("Behjatnia", customer.LastName);
    }
    #endregion

    #region Get All

    [Fact]
    public async void GetCustomer_Return_OkResult()
    {
        //Arrange
        var controller = new CustomerController();


        //Act
        var data = await controller.GetListAsync(new RequestDTO() { MaxResultCount = 20 });

        //Assert
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async void GetCustomer_Return_BadRequestResult()
    {
        //Arrange
        var controller = new CustomerController();


        //Act
        var data = await controller.GetListAsync(new RequestDTO());

        //Assert
        Assert.IsType<BadRequestResult>(data);
    }

    #endregion

    #region Upsert Customer
    [Fact]
    public async void InsertCustomer_Return_OkResult()
    {
        //Arrange
        var controller = new CustomerController();
        var customerUpsertDTO = new CustomerUpsertDTO
        {
            FirstName = "Ali",
            LastName = "Behjatnia",
            DateOfBirth = DateTime.Now,
            PhoneNumber = 989001096055,
            EmailAddress = "ali471@gmail.com",
            BankAccountNumber = "4567"
        };

        //Act
        var data = await controller.Upsert(customerUpsertDTO);

        //Assert
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async void UpdateCustomer_Return_OkResult()
    {
        //Arrange
        var controller = new CustomerController();
        var customerUpsertDTO = new CustomerUpsertDTO
        {
            Id = ExistedCutomerId,
            FirstName = "Abbas",
            LastName = "Behjatnia",
            DateOfBirth = DateTime.Now,
            PhoneNumber = 989171096055,
            EmailAddress = "sae471@gmail.com",
            BankAccountNumber = "123456"
        };

        //Act
        var data = await controller.Upsert(customerUpsertDTO);

        //Assert
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async void UpdateCustomer_Return_NotFoundResult()
    {
        //Arrange
        var controller = new CustomerController();
        var customerUpsertDTO = new CustomerUpsertDTO
        {
            Id = Guid.NewGuid(),
            FirstName = "Abbas",
            LastName = "Behjatnia",
            DateOfBirth = DateTime.Now,
            PhoneNumber = 989171096055,
            EmailAddress = "sae471@gmail.com",
            BankAccountNumber = "123456"
        };

        //Act
        var data = await controller.Upsert(customerUpsertDTO);

        //Assert
        Assert.IsType<NotFoundResult>(data);
    }

    [Fact]
    public void InsertCustomer_Return_DuplicatedException()
    {
        //Arrange
        var controller = new CustomerController();
        var customerUpsertDTO = new CustomerUpsertDTO
        {
            FirstName = "Abbas",
            LastName = "Behjatnia",
            DateOfBirth = new DateTime(1989, 2, 3),
            PhoneNumber = 989171096055,
            EmailAddress = "sae471@gmail.com",
            BankAccountNumber = "123456"
        };



        //Act
        var exception = Assert.Throws<System.AggregateException>(() => controller.Upsert(customerUpsertDTO).Result);

        //Assert
        Assert.NotNull(exception);
        Assert.Contains("Customer is already to exist!!", exception.Message);
    }

    [Fact]
    public void InsertCustomer_Return_PhonNumberFormatException()
    {
        //Arrange
        var controller = new CustomerController();
        var customerUpsertDTO = new CustomerUpsertDTO
        {
            FirstName = "Ali",
            LastName = "Behjatnia",
            DateOfBirth = DateTime.Now,
            PhoneNumber = 6055,
            EmailAddress = "ali471@gmail.com",
            BankAccountNumber = "4567"
        };



        //Act
        var exception = Assert.Throws<System.AggregateException>(() => controller.Upsert(customerUpsertDTO).Result);

        //Assert
        Assert.NotNull(exception);
        Assert.Contains("The phone number is not a valid mobile number format!!", exception.Message);
    }

    [Fact]
    public void InsertCustomer_Return_EmailAddressFormatException()
    {
        //Arrange
        var controller = new CustomerController();
        var customerUpsertDTO = new CustomerUpsertDTO
        {
            FirstName = "Ali",
            LastName = "Behjatnia",
            DateOfBirth = DateTime.Now,
            PhoneNumber = 989001096055,
            EmailAddress = "ali@",
            BankAccountNumber = "4567"
        };



        //Act
        var exception = Assert.Throws<System.AggregateException>(() => controller.Upsert(customerUpsertDTO).Result);

        //Assert
        Assert.NotNull(exception);
        Assert.Contains("The email address is incorrect fomrat!!", exception.Message);
    }

    #endregion

    #region Delete Post

    [Fact]
    public async void DeleteCustomer_Return_OkResult()
    {
        var controller = new CustomerController();
        var customerId = ExistedCutomerId;


        //Act
        var data = await controller.Delete(customerId);

        //Assert
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async void DeleteCustomer_Return_NotFoundResult()
    {
        //Arrange
        var controller = new CustomerController();
        var customerId = Guid.NewGuid();

        //Act
        var data = await controller.Delete(customerId);

        //Assert
        Assert.IsType<NotFoundResult>(data);
    }

    [Fact]
    public async void DeleteCustomer_Return_BadRequestResult()
    {
        //Arrange
        var controller = new CustomerController();
        var customerId = default(Guid);

        //Act
        var data = await controller.Delete(customerId);

        //Assert
        Assert.IsType<BadRequestResult>(data);
    }

    #endregion

}