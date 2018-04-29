using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using crossblog.Controllers;
using crossblog.Domain;
using crossblog.Model;
using crossblog.Repositories;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace crossblog.tests.Controllers
{
    public class CommentsControllerTests
    {
        private CommentsController _commentsController;
        private ArticlesController _articlesController;

        private Mock<IArticleRepository> _articleRepositoryMock = new Mock<IArticleRepository>();
        private Mock<ICommentRepository> _commentsRepositoryMock = new Mock<ICommentRepository>();

        public CommentsControllerTests()
        {
            _commentsController = new CommentsController(_articleRepositoryMock.Object, _commentsRepositoryMock.Object);
        }

        [Fact]
        public async Task Get_NotFound()
        {
            // Arrange
            _commentsRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Comment>(null));

            // Act
            var result = await _commentsController.Get(1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
        }

        //[Fact]
        //public async Task Get_ReturnsItem()
        //{
        //    // Arrange
        //    _commentsRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Comment>(Builder<Comment>.CreateNew().Build()));

        //    // Act
        //    var result = await _commentsController.Get(1);

        //    // Assert
        //    Assert.NotNull(result);

        //    var objectResult = result as OkObjectResult;
        //    Assert.NotNull(objectResult);

        //    var content = objectResult.Value as CommentModel;
        //    Assert.NotNull(content);

        //    Assert.Equal("Title1", content.Title);
        //}
    }
}
