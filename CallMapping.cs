using AutoMapper;
using JusticeOne.Data.Common.Models.CallForService;
using JusticeOne.Data.Tenant.Entities.Models;

namespace JusticeOne.Business.Calls.Mapping
{
    public class CallMapping : Profile
    {
        private class StatusConverter : IValueConverter<CallStatuses, string>
        {
            public string Convert(CallStatuses sourceMember, ResolutionContext context)
            {
                return sourceMember switch
                {
                    CallStatuses.New => "New",
                    CallStatuses.Dispatched => "Dispatched",
                    CallStatuses.Enroute => "Enroute",
                    CallStatuses.OnScene => "On Scene",
                    CallStatuses.OnHold => "On Hold",
                    CallStatuses.Scheduled => "Scheduled",
                    CallStatuses.Closed => "Closed",
                    _ => null,
                };
            }
        }

        private class CallToLocationConverter : IValueConverter<NewCallDto, Location>
        {
            public Location Convert(NewCallDto sourceMember, ResolutionContext context)
            {
                return new Location
                {
                    Description = sourceMember.LocationSummary,
                    Address = new Address
                    {
                        City = sourceMember.City,
                        County = sourceMember.County,
                        ZipCode = sourceMember.ZipCode,
                        StreetNumber = sourceMember.StreetNumber
                    }
                };
            }
        }

        private class TrafficToLocationConverter : IValueConverter<NewTrafficStopDto, Location>
        {
            public Location Convert(NewTrafficStopDto sourceMember, ResolutionContext context)
            {
                return new Location
                {
                    Description = sourceMember.LocationSummary,
                    Address = new Address()
                };
            }
        }

        public CallMapping()
        {
            CreateMap<CallDb, CallDto>()
                .ForMember(c => c.Complianant, opt => opt.MapFrom(s => s.Complainant != null ? $"{s.Complainant.FirstName} {s.Complainant.LastName}" : null))
                .ForMember(c => c.Status, opt => opt.ConvertUsing(new StatusConverter(), s => s.Status))
                .ForMember(c => c.Id, opt => opt.MapFrom(s => s.CallId))
                .ForMember(c => c.RecievedTime, opt => opt.MapFrom(s => s.ReceivedDateTime));

            CreateMap<NewCallDto, CallDb>()
                .ForMember(c => c.CallNumber, opt => opt.MapFrom(_ => 1122))
                .ForMember(c => c.CallTypeId, opt => opt.MapFrom(s => s.CallTypeId))
                .ForMember(c => c.OriginationId, opt => opt.MapFrom(s => s.OriginatedFromId))
                .ForMember(c => c.ReceivedDateTime, opt => opt.MapFrom(s => s.RecievedTime))
                .ForMember(c => c.Status, opt => opt.MapFrom(_ => CallStatuses.New))
                .ForMember(c => c.DispatcherId, opt => opt.MapFrom(s => s.DispatcherId))
                .ForMember(c => c.Location, opt => opt.ConvertUsing(new CallToLocationConverter(), s => s));

            CreateMap<NewTrafficStopDto, CallDb>()
                .ForMember(c => c.CallNumber, opt => opt.MapFrom(_ => 1122))
                .ForMember(c => c.CallTypeId, opt => opt.MapFrom(s => s.CallTypeId))
                .ForMember(c => c.OriginationId, opt => opt.MapFrom(s => s.OriginatedFromId))
                .ForMember(c => c.ReceivedDateTime, opt => opt.MapFrom(s => s.RecievedTime))
                .ForMember(c => c.Status, opt => opt.MapFrom(_ => CallStatuses.New))
                .ForMember(c => c.DispatcherId, opt => opt.MapFrom(s => s.DispatcherId))
                .ForMember(c => c.Location, opt => opt.ConvertUsing(new TrafficToLocationConverter(), s => s));
        }

    }
}
