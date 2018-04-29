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
            _articlesController = new ArticlesController(_articleRepositoryMock.Object);
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

        [Fact]
        public async Task Get_CommentsNotFound()
        {
            // Arrange
            _commentsRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Comment>(null));

            // Act
            var result = await _commentsController.Get(1,1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
        }

        [Fact]
        public async Task Get_ReturnsItem()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Article>(Builder<Article>.CreateNew().Build()));
           // _commentsRepositoryMock.Setup(m => m.Query()).Returns(Builder<Comment>.CreateListOfSize(1).Build().AsQueryable());
            var commentsDbSetMock = Builder<Comment>.CreateListOfSize(3).Build().ToAsyncDbSetMock();
            _commentsRepositoryMock.Setup(m => m.Query()).Returns(commentsDbSetMock.Object);

            // Act
            var result = await _commentsController.Get(1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            var content = objectResult.Value as CommentListModel;
            Assert.NotNull(content);
        }

        [Fact]
        public async Task Get_Comments()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Article>(Builder<Article>.CreateNew().Build()));
            var commentsDbSetMock = Builder<Comment>.CreateListOfSize(3).Build().ToAsyncDbSetMock();
            _commentsRepositoryMock.Setup(m => m.Query()).Returns(commentsDbSetMock.Object);

            // Act
            var result = await _commentsController.Get(1,1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            var content = objectResult.Value as CommentModel;
            Assert.NotNull(content);
        }

        [Fact]
        public async Task Post_Comment()
        {
            var comment = new Comment() { Title = "Post comment Test" };
            var commentModel = new CommentModel() { Title = "Post comment Test" };

            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Article>(Builder<Article>.CreateNew().Build()));
            _commentsRepositoryMock.Setup(m => m.InsertAsync(comment)).Returns(Task.FromResult<Comment>(comment));

            // Act
            var result = await _commentsController.Post(1,commentModel);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as CreatedResult;
            Assert.NotNull(objectResult);

            var content = objectResult.Value as CommentModel;
            Assert.NotNull(content);

            Assert.Equal(201, objectResult.StatusCode);

            Assert.Equal("Post comment Test", content.Title);
        }
    }
}
