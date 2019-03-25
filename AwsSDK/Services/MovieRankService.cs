using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwsSDK.Contracts;
using AwsSDK.Libs.Mappers;
using AwsSDK.Libs.Repositories;

namespace AwsSDK.Services
{
    public class MovieRankService : IMovieRankService
    {
        private readonly IMovieRankRepository _movieRankRepository;
        private readonly IMapper _map;

        public MovieRankService(IMovieRankRepository movieRankRepository, IMapper map)
        {
            _movieRankRepository = movieRankRepository;
            _map = map;
        }

        public async Task<IEnumerable<MovieResponse>> GetAllItemsFromDatabase()
        {
            var response = await _movieRankRepository.GetAllItems();

            return _map.ToMovieContract(response);
        }

        public async Task<MovieResponse> GetMovie(int userId, string movieName)
        {
            var response = await _movieRankRepository.GetMovie(userId, movieName);

            return _map.ToMovieContract(response);
        }

        public async Task<IEnumerable<MovieResponse>> GetUsersRankedMoviesByMovieTitle(int userId, string movieName)
        {
            var response = await _movieRankRepository.GetUsersRankedMoviesByMovieTitle(userId, movieName);

            return _map.ToMovieContract(response);
        }

        public async Task AddMovie(int userId, MovieRankRequest movieRankRequest)
        {
            var movieDb = _map.ToDocumentModel(userId, movieRankRequest);

            await _movieRankRepository.AddMovie(movieDb);
        }

        public async Task UpdateMovie(int userId, MovieUpdateRequest movieUpdateRequest)
        {
            var response = await GetMovie(userId, movieUpdateRequest.MovieName);

            var movieDb = _map.ToDocumentModel(userId, response, movieUpdateRequest);

            await _movieRankRepository.UpdateMovie(movieDb);
        }

        public async Task<MovieRankResponse> GetMovieRank(string movieName)
        {
            var response = await _movieRankRepository.GetMovieRank(movieName);

            var overallMovieRanking = Math.Round(response.Select(r => r["Ranking"].AsInt()).Average());

            return new MovieRankResponse()
            {
                MovieName = movieName,
                OverallRanking = overallMovieRanking
            };
        }
    }
}