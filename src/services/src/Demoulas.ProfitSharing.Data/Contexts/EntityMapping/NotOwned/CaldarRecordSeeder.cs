
using Demoulas.ProfitSharing.Data.Entities.NotOwned;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping.NotOwned;

public static class CaldarRecordSeeder
{
    public static void Seed(EntityTypeBuilder<CaldarRecord> builder)
    {
        builder.HasData(

            new CaldarRecord
            {
                AccWkendN = 41023,
                AccApWkend = 41030,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20041023,
                AccAltKeyNum = -66
            },

            new CaldarRecord
            {
                AccWkendN = 41030,
                AccApWkend = 41106,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20041030,
                AccAltKeyNum = -65
            },

            new CaldarRecord
            {
                AccWkendN = 41120,
                AccApWkend = 41127,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20041120,
                AccAltKeyNum = -62
            },

            new CaldarRecord
            {
                AccWkendN = 41204,
                AccApWkend = 41211,
                AccWeekN = 48,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 48,
                AccCln60Period = 12,
                AccCln61Week = 48,
                AccCln61Period = 12,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 12,
                AccWkend2N = 20041204,
                AccAltKeyNum = -60
            },

            new CaldarRecord
            {
                AccWkendN = 41211,
                AccApWkend = 41218,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20041211,
                AccAltKeyNum = -59
            },

            new CaldarRecord
            {
                AccWkendN = 41225,
                AccApWkend = 50101,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20041225,
                AccAltKeyNum = -57
            },

            new CaldarRecord
            {
                AccWkendN = 20824,
                AccApWkend = 20831,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20020824,
                AccAltKeyNum = -194
            },

            new CaldarRecord
            {
                AccWkendN = 20831,
                AccApWkend = 20907,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20020831,
                AccAltKeyNum = -193
            },

            new CaldarRecord
            {
                AccWkendN = 20216,
                AccApWkend = 20223,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20020216,
                AccAltKeyNum = -221
            },

            new CaldarRecord
            {
                AccWkendN = 20223,
                AccApWkend = 20302,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20020223,
                AccAltKeyNum = -220
            },

            new CaldarRecord
            {
                AccWkendN = 20309,
                AccApWkend = 20316,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20020309,
                AccAltKeyNum = -218
            },

            new CaldarRecord
            {
                AccWkendN = 20323,
                AccApWkend = 20330,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20020323,
                AccAltKeyNum = -216
            },

            new CaldarRecord
            {
                AccWkendN = 20330,
                AccApWkend = 20406,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20020330,
                AccAltKeyNum = -215
            },

            new CaldarRecord
            {
                AccWkendN = 20406,
                AccApWkend = 20413,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20020406,
                AccAltKeyNum = -214
            },

            new CaldarRecord
            {
                AccWkendN = 20420,
                AccApWkend = 20427,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20020420,
                AccAltKeyNum = -212
            },

            new CaldarRecord
            {
                AccWkendN = 20427,
                AccApWkend = 20504,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20020427,
                AccAltKeyNum = -211
            },

            new CaldarRecord
            {
                AccWkendN = 20511,
                AccApWkend = 20518,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20020511,
                AccAltKeyNum = -209
            },

            new CaldarRecord
            {
                AccWkendN = 20601,
                AccApWkend = 20608,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20020601,
                AccAltKeyNum = -206
            },

            new CaldarRecord
            {
                AccWkendN = 20615,
                AccApWkend = 20622,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20020615,
                AccAltKeyNum = -204
            },

            new CaldarRecord
            {
                AccWkendN = 20706,
                AccApWkend = 20713,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20020706,
                AccAltKeyNum = -201
            },

            new CaldarRecord
            {
                AccWkendN = 10804,
                AccApWkend = 10811,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20010804,
                AccAltKeyNum = -257
            },

            new CaldarRecord
            {
                AccWkendN = 10908,
                AccApWkend = 10915,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20010908,
                AccAltKeyNum = -252
            },

            new CaldarRecord
            {
                AccWkendN = 10915,
                AccApWkend = 10922,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20010915,
                AccAltKeyNum = -251
            },

            new CaldarRecord
            {
                AccWkendN = 11020,
                AccApWkend = 11027,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20011020,
                AccAltKeyNum = -246
            },

            new CaldarRecord
            {
                AccWkendN = 11103,
                AccApWkend = 11110,
                AccWeekN = 44,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 44,
                AccCln60Period = 10,
                AccCln61Week = 44,
                AccCln61Period = 10,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 10,
                AccWkend2N = 20011103,
                AccAltKeyNum = -244
            },

            new CaldarRecord
            {
                AccWkendN = 11124,
                AccApWkend = 11201,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20011124,
                AccAltKeyNum = -241
            },

            new CaldarRecord
            {
                AccWkendN = 11215,
                AccApWkend = 11222,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20011215,
                AccAltKeyNum = -238
            },

            new CaldarRecord
            {
                AccWkendN = 10217,
                AccApWkend = 10224,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20010217,
                AccAltKeyNum = -281
            },

            new CaldarRecord
            {
                AccWkendN = 10324,
                AccApWkend = 10331,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20010324,
                AccAltKeyNum = -276
            },

            new CaldarRecord
            {
                AccWkendN = 10526,
                AccApWkend = 10602,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20010526,
                AccAltKeyNum = -267
            },

            new CaldarRecord
            {
                AccWkendN = 10623,
                AccApWkend = 10630,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20010623,
                AccAltKeyNum = -263
            },

            new CaldarRecord
            {
                AccWkendN = 10106,
                AccApWkend = 10113,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20010106,
                AccAltKeyNum = -287
            },

            new CaldarRecord
            {
                AccWkendN = 10113,
                AccApWkend = 10120,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20010113,
                AccAltKeyNum = -286
            },

            new CaldarRecord
            {
                AccWkendN = 20928,
                AccApWkend = 21005,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20020928,
                AccAltKeyNum = -189
            },

            new CaldarRecord
            {
                AccWkendN = 21019,
                AccApWkend = 21026,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20021019,
                AccAltKeyNum = -186
            },

            new CaldarRecord
            {
                AccWkendN = 21102,
                AccApWkend = 21109,
                AccWeekN = 44,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 44,
                AccCln60Period = 10,
                AccCln61Week = 44,
                AccCln61Period = 10,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 10,
                AccWkend2N = 20021102,
                AccAltKeyNum = -184
            },

            new CaldarRecord
            {
                AccWkendN = 21123,
                AccApWkend = 21130,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20021123,
                AccAltKeyNum = -181
            },

            new CaldarRecord
            {
                AccWkendN = 21207,
                AccApWkend = 21214,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20021207,
                AccAltKeyNum = -179
            },

            new CaldarRecord
            {
                AccWkendN = 50226,
                AccApWkend = 50305,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20050226,
                AccAltKeyNum = -40
            },

            new CaldarRecord
            {
                AccWkendN = 50305,
                AccApWkend = 50312,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 20050305,
                AccAltKeyNum = -39
            },

            new CaldarRecord
            {
                AccWkendN = 50312,
                AccApWkend = 50319,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20050312,
                AccAltKeyNum = -38
            },

            new CaldarRecord
            {
                AccWkendN = 50319,
                AccApWkend = 50326,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20050319,
                AccAltKeyNum = -37
            },

            new CaldarRecord
            {
                AccWkendN = 50409,
                AccApWkend = 50416,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20050409,
                AccAltKeyNum = -34
            },

            new CaldarRecord
            {
                AccWkendN = 50416,
                AccApWkend = 50423,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20050416,
                AccAltKeyNum = -33
            },

            new CaldarRecord
            {
                AccWkendN = 50430,
                AccApWkend = 50507,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20050430,
                AccAltKeyNum = -31
            },

            new CaldarRecord
            {
                AccWkendN = 50507,
                AccApWkend = 50514,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20050507,
                AccAltKeyNum = -30
            },

            new CaldarRecord
            {
                AccWkendN = 50514,
                AccApWkend = 50521,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20050514,
                AccAltKeyNum = -29
            },

            new CaldarRecord
            {
                AccWkendN = 40807,
                AccApWkend = 40814,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20040807,
                AccAltKeyNum = -77
            },

            new CaldarRecord
            {
                AccWkendN = 40814,
                AccApWkend = 40821,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20040814,
                AccAltKeyNum = -76
            },

            new CaldarRecord
            {
                AccWkendN = 40821,
                AccApWkend = 40828,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20040821,
                AccAltKeyNum = -75
            },

            new CaldarRecord
            {
                AccWkendN = 40925,
                AccApWkend = 41002,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20040925,
                AccAltKeyNum = -70
            },

            new CaldarRecord
            {
                AccWkendN = 41106,
                AccApWkend = 41113,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 20041106,
                AccAltKeyNum = -64
            },

            new CaldarRecord
            {
                AccWkendN = 41113,
                AccApWkend = 41120,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20041113,
                AccAltKeyNum = -63
            },

            new CaldarRecord
            {
                AccWkendN = 41127,
                AccApWkend = 41204,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20041127,
                AccAltKeyNum = -61
            },

            new CaldarRecord
            {
                AccWkendN = 41218,
                AccApWkend = 41225,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20041218,
                AccAltKeyNum = -58
            },

            new CaldarRecord
            {
                AccWkendN = 50101,
                AccApWkend = 50108,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20050101,
                AccAltKeyNum = -56
            },

            new CaldarRecord
            {
                AccWkendN = 50115,
                AccApWkend = 50122,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20050115,
                AccAltKeyNum = -46
            },

            new CaldarRecord
            {
                AccWkendN = 40207,
                AccApWkend = 40214,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20040207,
                AccAltKeyNum = -103
            },

            new CaldarRecord
            {
                AccWkendN = 40214,
                AccApWkend = 40221,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20040214,
                AccAltKeyNum = -102
            },

            new CaldarRecord
            {
                AccWkendN = 40306,
                AccApWkend = 40313,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 20040306,
                AccAltKeyNum = -99
            },

            new CaldarRecord
            {
                AccWkendN = 40313,
                AccApWkend = 40320,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20040313,
                AccAltKeyNum = -98
            },

            new CaldarRecord
            {
                AccWkendN = 40320,
                AccApWkend = 40327,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20040320,
                AccAltKeyNum = -97
            },

            new CaldarRecord
            {
                AccWkendN = 40515,
                AccApWkend = 40522,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20040515,
                AccAltKeyNum = -89
            },

            new CaldarRecord
            {
                AccWkendN = 40529,
                AccApWkend = 40605,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20040529,
                AccAltKeyNum = -87
            },

            new CaldarRecord
            {
                AccWkendN = 40605,
                AccApWkend = 40612,
                AccWeekN = 22,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 22,
                AccCln60Period = 6,
                AccCln61Week = 22,
                AccCln61Period = 6,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 6,
                AccWkend2N = 20040605,
                AccAltKeyNum = -86
            },

            new CaldarRecord
            {
                AccWkendN = 40703,
                AccApWkend = 40710,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20040703,
                AccAltKeyNum = -82
            },

            new CaldarRecord
            {
                AccWkendN = 40717,
                AccApWkend = 40724,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20040717,
                AccAltKeyNum = -80
            },

            new CaldarRecord
            {
                AccWkendN = 40724,
                AccApWkend = 40731,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20040724,
                AccAltKeyNum = -79
            },

            new CaldarRecord
            {
                AccWkendN = 30809,
                AccApWkend = 30816,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20030809,
                AccAltKeyNum = -136
            },

            new CaldarRecord
            {
                AccWkendN = 30823,
                AccApWkend = 30830,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20030823,
                AccAltKeyNum = -134
            },

            new CaldarRecord
            {
                AccWkendN = 30830,
                AccApWkend = 30906,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20030830,
                AccAltKeyNum = -133
            },

            new CaldarRecord
            {
                AccWkendN = 31018,
                AccApWkend = 31025,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20031018,
                AccAltKeyNum = -126
            },

            new CaldarRecord
            {
                AccWkendN = 31108,
                AccApWkend = 31115,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20031108,
                AccAltKeyNum = -123
            },

            new CaldarRecord
            {
                AccWkendN = 31213,
                AccApWkend = 31220,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20031213,
                AccAltKeyNum = -118
            },

            new CaldarRecord
            {
                AccWkendN = 31220,
                AccApWkend = 31227,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20031220,
                AccAltKeyNum = -117
            },

            new CaldarRecord
            {
                AccWkendN = 40117,
                AccApWkend = 40124,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20040117,
                AccAltKeyNum = -106
            },

            new CaldarRecord
            {
                AccWkendN = 40131,
                AccApWkend = 40207,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20040131,
                AccAltKeyNum = -104
            },

            new CaldarRecord
            {
                AccWkendN = 50108,
                AccApWkend = 50115,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20050108,
                AccAltKeyNum = -47
            },

            new CaldarRecord
            {
                AccWkendN = 50122,
                AccApWkend = 50129,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20050122,
                AccAltKeyNum = -45
            },

            new CaldarRecord
            {
                AccWkendN = 50129,
                AccApWkend = 50205,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20050129,
                AccAltKeyNum = -44
            },

            new CaldarRecord
            {
                AccWkendN = 50205,
                AccApWkend = 50212,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20050205,
                AccAltKeyNum = -43
            },

            new CaldarRecord
            {
                AccWkendN = 50212,
                AccApWkend = 50219,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20050212,
                AccAltKeyNum = -42
            },

            new CaldarRecord
            {
                AccWkendN = 50219,
                AccApWkend = 50226,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20050219,
                AccAltKeyNum = -41
            },

            new CaldarRecord
            {
                AccWkendN = 30628,
                AccApWkend = 30705,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20030628,
                AccAltKeyNum = -142
            },

            new CaldarRecord
            {
                AccWkendN = 30705,
                AccApWkend = 30712,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20030705,
                AccAltKeyNum = -141
            },

            new CaldarRecord
            {
                AccWkendN = 30726,
                AccApWkend = 30802,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20030726,
                AccAltKeyNum = -138
            },

            new CaldarRecord
            {
                AccWkendN = 50326,
                AccApWkend = 50402,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20050326,
                AccAltKeyNum = -36
            },

            new CaldarRecord
            {
                AccWkendN = 50402,
                AccApWkend = 50409,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20050402,
                AccAltKeyNum = -35
            },

            new CaldarRecord
            {
                AccWkendN = 50423,
                AccApWkend = 50430,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20050423,
                AccAltKeyNum = -32
            },

            new CaldarRecord
            {
                AccWkendN = 50521,
                AccApWkend = 50528,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20050521,
                AccAltKeyNum = -28
            },

            new CaldarRecord
            {
                AccWkendN = 50604,
                AccApWkend = 50611,
                AccWeekN = 22,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 22,
                AccCln60Period = 6,
                AccCln61Week = 22,
                AccCln61Period = 6,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 6,
                AccWkend2N = 20050604,
                AccAltKeyNum = -26
            },

            new CaldarRecord
            {
                AccWkendN = 50611,
                AccApWkend = 50618,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20050611,
                AccAltKeyNum = -25
            },

            new CaldarRecord
            {
                AccWkendN = 50618,
                AccApWkend = 50625,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20050618,
                AccAltKeyNum = -24
            },

            new CaldarRecord
            {
                AccWkendN = 50625,
                AccApWkend = 50702,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20050625,
                AccAltKeyNum = -23
            },

            new CaldarRecord
            {
                AccWkendN = 50716,
                AccApWkend = 50723,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20050716,
                AccAltKeyNum = -20
            },

            new CaldarRecord
            {
                AccWkendN = 50723,
                AccApWkend = 50730,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20050723,
                AccAltKeyNum = -19
            },

            new CaldarRecord
            {
                AccWkendN = 50730,
                AccApWkend = 50806,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20050730,
                AccAltKeyNum = -18
            },

            new CaldarRecord
            {
                AccWkendN = 50813,
                AccApWkend = 50820,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20050813,
                AccAltKeyNum = -16
            },

            new CaldarRecord
            {
                AccWkendN = 50820,
                AccApWkend = 50827,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20050820,
                AccAltKeyNum = -15
            },

            new CaldarRecord
            {
                AccWkendN = 50827,
                AccApWkend = 50903,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20050827,
                AccAltKeyNum = -14
            },

            new CaldarRecord
            {
                AccWkendN = 50903,
                AccApWkend = 50910,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20050903,
                AccAltKeyNum = -13
            },

            new CaldarRecord
            {
                AccWkendN = 50910,
                AccApWkend = 50917,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20050910,
                AccAltKeyNum = -12
            },

            new CaldarRecord
            {
                AccWkendN = 50924,
                AccApWkend = 51001,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20050924,
                AccAltKeyNum = -10
            },

            new CaldarRecord
            {
                AccWkendN = 51001,
                AccApWkend = 51008,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20051001,
                AccAltKeyNum = -9
            },

            new CaldarRecord
            {
                AccWkendN = 51015,
                AccApWkend = 51022,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20051015,
                AccAltKeyNum = -7
            },

            new CaldarRecord
            {
                AccWkendN = 51022,
                AccApWkend = 51029,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20051022,
                AccAltKeyNum = -6
            },

            new CaldarRecord
            {
                AccWkendN = 51105,
                AccApWkend = 51112,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 20051105,
                AccAltKeyNum = -4
            },

            new CaldarRecord
            {
                AccWkendN = 51112,
                AccApWkend = 51119,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20051112,
                AccAltKeyNum = -3
            },

            new CaldarRecord
            {
                AccWkendN = 51119,
                AccApWkend = 51126,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20051119,
                AccAltKeyNum = -2
            },

            new CaldarRecord
            {
                AccWkendN = 51203,
                AccApWkend = 51210,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20051203,
                AccAltKeyNum = 0
            },

            new CaldarRecord
            {
                AccWkendN = 51210,
                AccApWkend = 51217,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20051210,
                AccAltKeyNum = 1
            },

            new CaldarRecord
            {
                AccWkendN = 51217,
                AccApWkend = 51224,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20051217,
                AccAltKeyNum = 2
            },

            new CaldarRecord
            {
                AccWkendN = 51224,
                AccApWkend = 51231,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20051224,
                AccAltKeyNum = 3
            },

            new CaldarRecord
            {
                AccWkendN = 51231,
                AccApWkend = 60107,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20051231,
                AccAltKeyNum = 4
            },

            new CaldarRecord
            {
                AccWkendN = 60107,
                AccApWkend = 60114,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20060107,
                AccAltKeyNum = 13
            },

            new CaldarRecord
            {
                AccWkendN = 60114,
                AccApWkend = 60121,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20060114,
                AccAltKeyNum = 14
            },

            new CaldarRecord
            {
                AccWkendN = 60121,
                AccApWkend = 60128,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20060121,
                AccAltKeyNum = 15
            },

            new CaldarRecord
            {
                AccWkendN = 60128,
                AccApWkend = 60204,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20060128,
                AccAltKeyNum = 16
            },

            new CaldarRecord
            {
                AccWkendN = 60204,
                AccApWkend = 60211,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20060204,
                AccAltKeyNum = 17
            },

            new CaldarRecord
            {
                AccWkendN = 60211,
                AccApWkend = 60218,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20060211,
                AccAltKeyNum = 18
            },

            new CaldarRecord
            {
                AccWkendN = 60218,
                AccApWkend = 60225,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20060218,
                AccAltKeyNum = 19
            },

            new CaldarRecord
            {
                AccWkendN = 60225,
                AccApWkend = 60304,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20060225,
                AccAltKeyNum = 20
            },

            new CaldarRecord
            {
                AccWkendN = 60304,
                AccApWkend = 60311,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 20060304,
                AccAltKeyNum = 21
            },

            new CaldarRecord
            {
                AccWkendN = 60311,
                AccApWkend = 60318,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20060311,
                AccAltKeyNum = 22
            },

            new CaldarRecord
            {
                AccWkendN = 60318,
                AccApWkend = 60325,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20060318,
                AccAltKeyNum = 23
            },

            new CaldarRecord
            {
                AccWkendN = 60325,
                AccApWkend = 60401,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20060325,
                AccAltKeyNum = 24
            },

            new CaldarRecord
            {
                AccWkendN = 60401,
                AccApWkend = 60408,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20060401,
                AccAltKeyNum = 25
            },

            new CaldarRecord
            {
                AccWkendN = 60408,
                AccApWkend = 60415,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20060408,
                AccAltKeyNum = 26
            },

            new CaldarRecord
            {
                AccWkendN = 60415,
                AccApWkend = 60422,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20060415,
                AccAltKeyNum = 27
            },

            new CaldarRecord
            {
                AccWkendN = 60422,
                AccApWkend = 60429,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20060422,
                AccAltKeyNum = 28
            },

            new CaldarRecord
            {
                AccWkendN = 60429,
                AccApWkend = 60506,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20060429,
                AccAltKeyNum = 29
            },

            new CaldarRecord
            {
                AccWkendN = 60506,
                AccApWkend = 60513,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20060506,
                AccAltKeyNum = 30
            },

            new CaldarRecord
            {
                AccWkendN = 60513,
                AccApWkend = 60520,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20060513,
                AccAltKeyNum = 31
            },

            new CaldarRecord
            {
                AccWkendN = 60520,
                AccApWkend = 60527,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20060520,
                AccAltKeyNum = 32
            },

            new CaldarRecord
            {
                AccWkendN = 60527,
                AccApWkend = 60603,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20060527,
                AccAltKeyNum = 33
            },

            new CaldarRecord
            {
                AccWkendN = 60603,
                AccApWkend = 60610,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20060603,
                AccAltKeyNum = 34
            },

            new CaldarRecord
            {
                AccWkendN = 60610,
                AccApWkend = 60617,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20060610,
                AccAltKeyNum = 35
            },

            new CaldarRecord
            {
                AccWkendN = 60617,
                AccApWkend = 60624,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20060617,
                AccAltKeyNum = 36
            },

            new CaldarRecord
            {
                AccWkendN = 60624,
                AccApWkend = 60701,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20060624,
                AccAltKeyNum = 37
            },

            new CaldarRecord
            {
                AccWkendN = 60701,
                AccApWkend = 60708,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20060701,
                AccAltKeyNum = 38
            },

            new CaldarRecord
            {
                AccWkendN = 60708,
                AccApWkend = 60715,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20060708,
                AccAltKeyNum = 39
            },

            new CaldarRecord
            {
                AccWkendN = 60715,
                AccApWkend = 60722,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20060715,
                AccAltKeyNum = 40
            },

            new CaldarRecord
            {
                AccWkendN = 60722,
                AccApWkend = 60729,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20060722,
                AccAltKeyNum = 41
            },

            new CaldarRecord
            {
                AccWkendN = 60729,
                AccApWkend = 60805,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20060729,
                AccAltKeyNum = 42
            },

            new CaldarRecord
            {
                AccWkendN = 60805,
                AccApWkend = 60812,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20060805,
                AccAltKeyNum = 43
            },

            new CaldarRecord
            {
                AccWkendN = 60812,
                AccApWkend = 60819,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20060812,
                AccAltKeyNum = 44
            },

            new CaldarRecord
            {
                AccWkendN = 60819,
                AccApWkend = 60826,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20060819,
                AccAltKeyNum = 45
            },

            new CaldarRecord
            {
                AccWkendN = 60826,
                AccApWkend = 60902,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20060826,
                AccAltKeyNum = 46
            },

            new CaldarRecord
            {
                AccWkendN = 60902,
                AccApWkend = 60909,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20060902,
                AccAltKeyNum = 47
            },

            new CaldarRecord
            {
                AccWkendN = 60909,
                AccApWkend = 60916,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20060909,
                AccAltKeyNum = 48
            },

            new CaldarRecord
            {
                AccWkendN = 60916,
                AccApWkend = 60923,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20060916,
                AccAltKeyNum = 49
            },

            new CaldarRecord
            {
                AccWkendN = 60923,
                AccApWkend = 60930,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20060923,
                AccAltKeyNum = 50
            },

            new CaldarRecord
            {
                AccWkendN = 60930,
                AccApWkend = 61007,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20060930,
                AccAltKeyNum = 51
            },

            new CaldarRecord
            {
                AccWkendN = 61007,
                AccApWkend = 61014,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20061007,
                AccAltKeyNum = 52
            },

            new CaldarRecord
            {
                AccWkendN = 61014,
                AccApWkend = 61021,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20061014,
                AccAltKeyNum = 53
            },

            new CaldarRecord
            {
                AccWkendN = 61021,
                AccApWkend = 61028,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20061021,
                AccAltKeyNum = 54
            },

            new CaldarRecord
            {
                AccWkendN = 61028,
                AccApWkend = 61104,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20061028,
                AccAltKeyNum = 55
            },

            new CaldarRecord
            {
                AccWkendN = 61104,
                AccApWkend = 61111,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 20061104,
                AccAltKeyNum = 56
            },

            new CaldarRecord
            {
                AccWkendN = 61111,
                AccApWkend = 61118,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20061111,
                AccAltKeyNum = 57
            },

            new CaldarRecord
            {
                AccWkendN = 61118,
                AccApWkend = 61125,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20061118,
                AccAltKeyNum = 58
            },

            new CaldarRecord
            {
                AccWkendN = 61125,
                AccApWkend = 61202,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20061125,
                AccAltKeyNum = 59
            },

            new CaldarRecord
            {
                AccWkendN = 61202,
                AccApWkend = 61209,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20061202,
                AccAltKeyNum = 60
            },

            new CaldarRecord
            {
                AccWkendN = 61209,
                AccApWkend = 61216,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20061209,
                AccAltKeyNum = 61
            },

            new CaldarRecord
            {
                AccWkendN = 61216,
                AccApWkend = 61223,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20061216,
                AccAltKeyNum = 62
            },

            new CaldarRecord
            {
                AccWkendN = 61223,
                AccApWkend = 61230,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20061223,
                AccAltKeyNum = 63
            },

            new CaldarRecord
            {
                AccWkendN = 61230,
                AccApWkend = 70106,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20061230,
                AccAltKeyNum = 64
            },

            new CaldarRecord
            {
                AccWkendN = 70106,
                AccApWkend = 70113,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20070106,
                AccAltKeyNum = 73
            },

            new CaldarRecord
            {
                AccWkendN = 70113,
                AccApWkend = 70120,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20070113,
                AccAltKeyNum = 74
            },

            new CaldarRecord
            {
                AccWkendN = 70120,
                AccApWkend = 70127,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20070120,
                AccAltKeyNum = 75
            },

            new CaldarRecord
            {
                AccWkendN = 70127,
                AccApWkend = 70203,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20070127,
                AccAltKeyNum = 76
            },

            new CaldarRecord
            {
                AccWkendN = 70203,
                AccApWkend = 70210,
                AccWeekN = 5,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 5,
                AccCln60Period = 1,
                AccCln61Week = 5,
                AccCln61Period = 1,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 1,
                AccWkend2N = 20070203,
                AccAltKeyNum = 77
            },

            new CaldarRecord
            {
                AccWkendN = 70210,
                AccApWkend = 70217,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20070210,
                AccAltKeyNum = 78
            },

            new CaldarRecord
            {
                AccWkendN = 70217,
                AccApWkend = 70224,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20070217,
                AccAltKeyNum = 79
            },

            new CaldarRecord
            {
                AccWkendN = 70224,
                AccApWkend = 70303,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20070224,
                AccAltKeyNum = 80
            },

            new CaldarRecord
            {
                AccWkendN = 70303,
                AccApWkend = 70310,
                AccWeekN = 9,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 9,
                AccCln60Period = 2,
                AccCln61Week = 9,
                AccCln61Period = 2,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 2,
                AccWkend2N = 20070303,
                AccAltKeyNum = 81
            },

            new CaldarRecord
            {
                AccWkendN = 70310,
                AccApWkend = 70317,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20070310,
                AccAltKeyNum = 82
            },

            new CaldarRecord
            {
                AccWkendN = 70317,
                AccApWkend = 70324,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20070317,
                AccAltKeyNum = 83
            },

            new CaldarRecord
            {
                AccWkendN = 70324,
                AccApWkend = 70331,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20070324,
                AccAltKeyNum = 84
            },

            new CaldarRecord
            {
                AccWkendN = 70331,
                AccApWkend = 70407,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20070331,
                AccAltKeyNum = 85
            },

            new CaldarRecord
            {
                AccWkendN = 70407,
                AccApWkend = 70414,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20070407,
                AccAltKeyNum = 86
            },

            new CaldarRecord
            {
                AccWkendN = 70414,
                AccApWkend = 70421,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20070414,
                AccAltKeyNum = 87
            },

            new CaldarRecord
            {
                AccWkendN = 70421,
                AccApWkend = 70428,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20070421,
                AccAltKeyNum = 88
            },

            new CaldarRecord
            {
                AccWkendN = 70428,
                AccApWkend = 70505,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20070428,
                AccAltKeyNum = 89
            },

            new CaldarRecord
            {
                AccWkendN = 70505,
                AccApWkend = 70512,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20070505,
                AccAltKeyNum = 90
            },

            new CaldarRecord
            {
                AccWkendN = 70512,
                AccApWkend = 70519,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20070512,
                AccAltKeyNum = 91
            },

            new CaldarRecord
            {
                AccWkendN = 70519,
                AccApWkend = 70526,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20070519,
                AccAltKeyNum = 92
            },

            new CaldarRecord
            {
                AccWkendN = 70526,
                AccApWkend = 70602,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20070526,
                AccAltKeyNum = 93
            },

            new CaldarRecord
            {
                AccWkendN = 70602,
                AccApWkend = 70609,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20070602,
                AccAltKeyNum = 94
            },

            new CaldarRecord
            {
                AccWkendN = 70609,
                AccApWkend = 70616,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20070609,
                AccAltKeyNum = 95
            },

            new CaldarRecord
            {
                AccWkendN = 70616,
                AccApWkend = 70623,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20070616,
                AccAltKeyNum = 96
            },

            new CaldarRecord
            {
                AccWkendN = 70623,
                AccApWkend = 70630,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20070623,
                AccAltKeyNum = 97
            },

            new CaldarRecord
            {
                AccWkendN = 70630,
                AccApWkend = 70707,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20070630,
                AccAltKeyNum = 98
            },

            new CaldarRecord
            {
                AccWkendN = 70707,
                AccApWkend = 70714,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20070707,
                AccAltKeyNum = 99
            },

            new CaldarRecord
            {
                AccWkendN = 70714,
                AccApWkend = 70721,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20070714,
                AccAltKeyNum = 100
            },

            new CaldarRecord
            {
                AccWkendN = 70721,
                AccApWkend = 70728,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20070721,
                AccAltKeyNum = 101
            },

            new CaldarRecord
            {
                AccWkendN = 70728,
                AccApWkend = 70804,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20070728,
                AccAltKeyNum = 102
            },

            new CaldarRecord
            {
                AccWkendN = 70804,
                AccApWkend = 70811,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20070804,
                AccAltKeyNum = 103
            },

            new CaldarRecord
            {
                AccWkendN = 70811,
                AccApWkend = 70818,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20070811,
                AccAltKeyNum = 104
            },

            new CaldarRecord
            {
                AccWkendN = 70818,
                AccApWkend = 70825,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20070818,
                AccAltKeyNum = 105
            },

            new CaldarRecord
            {
                AccWkendN = 70825,
                AccApWkend = 70901,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20070825,
                AccAltKeyNum = 106
            },

            new CaldarRecord
            {
                AccWkendN = 70901,
                AccApWkend = 70908,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20070901,
                AccAltKeyNum = 107
            },

            new CaldarRecord
            {
                AccWkendN = 70908,
                AccApWkend = 70915,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20070908,
                AccAltKeyNum = 108
            },

            new CaldarRecord
            {
                AccWkendN = 70915,
                AccApWkend = 70922,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20070915,
                AccAltKeyNum = 109
            },

            new CaldarRecord
            {
                AccWkendN = 70922,
                AccApWkend = 70929,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20070922,
                AccAltKeyNum = 110
            },

            new CaldarRecord
            {
                AccWkendN = 70929,
                AccApWkend = 71006,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20070929,
                AccAltKeyNum = 111
            },

            new CaldarRecord
            {
                AccWkendN = 71006,
                AccApWkend = 71013,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20071006,
                AccAltKeyNum = 112
            },

            new CaldarRecord
            {
                AccWkendN = 71013,
                AccApWkend = 71020,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20071013,
                AccAltKeyNum = 113
            },

            new CaldarRecord
            {
                AccWkendN = 71020,
                AccApWkend = 71027,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20071020,
                AccAltKeyNum = 114
            },

            new CaldarRecord
            {
                AccWkendN = 71027,
                AccApWkend = 71103,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20071027,
                AccAltKeyNum = 115
            },

            new CaldarRecord
            {
                AccWkendN = 71103,
                AccApWkend = 71110,
                AccWeekN = 44,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 44,
                AccCln60Period = 10,
                AccCln61Week = 44,
                AccCln61Period = 10,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 10,
                AccWkend2N = 20071103,
                AccAltKeyNum = 116
            },

            new CaldarRecord
            {
                AccWkendN = 71110,
                AccApWkend = 71117,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20071110,
                AccAltKeyNum = 117
            },

            new CaldarRecord
            {
                AccWkendN = 71117,
                AccApWkend = 71124,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20071117,
                AccAltKeyNum = 118
            },

            new CaldarRecord
            {
                AccWkendN = 71124,
                AccApWkend = 71201,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20071124,
                AccAltKeyNum = 119
            },

            new CaldarRecord
            {
                AccWkendN = 71201,
                AccApWkend = 71208,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20071201,
                AccAltKeyNum = 120
            },

            new CaldarRecord
            {
                AccWkendN = 71208,
                AccApWkend = 71215,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20071208,
                AccAltKeyNum = 121
            },

            new CaldarRecord
            {
                AccWkendN = 71215,
                AccApWkend = 71222,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20071215,
                AccAltKeyNum = 122
            },

            new CaldarRecord
            {
                AccWkendN = 71222,
                AccApWkend = 71229,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20071222,
                AccAltKeyNum = 123
            },

            new CaldarRecord
            {
                AccWkendN = 71229,
                AccApWkend = 80105,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20071229,
                AccAltKeyNum = 124
            },

            new CaldarRecord
            {
                AccWkendN = 80105,
                AccApWkend = 80112,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20080105,
                AccAltKeyNum = 133
            },

            new CaldarRecord
            {
                AccWkendN = 80112,
                AccApWkend = 80119,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20080112,
                AccAltKeyNum = 134
            },

            new CaldarRecord
            {
                AccWkendN = 80119,
                AccApWkend = 80126,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20080119,
                AccAltKeyNum = 135
            },

            new CaldarRecord
            {
                AccWkendN = 80126,
                AccApWkend = 80202,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20080126,
                AccAltKeyNum = 136
            },

            new CaldarRecord
            {
                AccWkendN = 80202,
                AccApWkend = 80209,
                AccWeekN = 5,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 5,
                AccCln60Period = 1,
                AccCln61Week = 5,
                AccCln61Period = 1,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 1,
                AccWkend2N = 20080202,
                AccAltKeyNum = 137
            },

            new CaldarRecord
            {
                AccWkendN = 80209,
                AccApWkend = 80216,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20080209,
                AccAltKeyNum = 138
            },

            new CaldarRecord
            {
                AccWkendN = 80216,
                AccApWkend = 80223,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20080216,
                AccAltKeyNum = 139
            },

            new CaldarRecord
            {
                AccWkendN = 80223,
                AccApWkend = 80301,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20080223,
                AccAltKeyNum = 140
            },

            new CaldarRecord
            {
                AccWkendN = 80301,
                AccApWkend = 80308,
                AccWeekN = 9,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 9,
                AccCln60Period = 2,
                AccCln61Week = 9,
                AccCln61Period = 2,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 2,
                AccWkend2N = 20080301,
                AccAltKeyNum = 141
            },

            new CaldarRecord
            {
                AccWkendN = 80308,
                AccApWkend = 80315,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20080308,
                AccAltKeyNum = 142
            },

            new CaldarRecord
            {
                AccWkendN = 80315,
                AccApWkend = 80322,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20080315,
                AccAltKeyNum = 143
            },

            new CaldarRecord
            {
                AccWkendN = 80322,
                AccApWkend = 80329,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20080322,
                AccAltKeyNum = 144
            },

            new CaldarRecord
            {
                AccWkendN = 80329,
                AccApWkend = 80405,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20080329,
                AccAltKeyNum = 145
            },

            new CaldarRecord
            {
                AccWkendN = 80405,
                AccApWkend = 80412,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20080405,
                AccAltKeyNum = 146
            },

            new CaldarRecord
            {
                AccWkendN = 80412,
                AccApWkend = 80419,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20080412,
                AccAltKeyNum = 147
            },

            new CaldarRecord
            {
                AccWkendN = 80419,
                AccApWkend = 80426,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20080419,
                AccAltKeyNum = 148
            },

            new CaldarRecord
            {
                AccWkendN = 80426,
                AccApWkend = 80503,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20080426,
                AccAltKeyNum = 149
            },

            new CaldarRecord
            {
                AccWkendN = 80503,
                AccApWkend = 80510,
                AccWeekN = 18,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 18,
                AccCln60Period = 4,
                AccCln61Week = 18,
                AccCln61Period = 4,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 4,
                AccWkend2N = 20080503,
                AccAltKeyNum = 150
            },

            new CaldarRecord
            {
                AccWkendN = 80510,
                AccApWkend = 80517,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20080510,
                AccAltKeyNum = 151
            },

            new CaldarRecord
            {
                AccWkendN = 80517,
                AccApWkend = 80524,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20080517,
                AccAltKeyNum = 152
            },

            new CaldarRecord
            {
                AccWkendN = 80524,
                AccApWkend = 80531,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20080524,
                AccAltKeyNum = 153
            },

            new CaldarRecord
            {
                AccWkendN = 80531,
                AccApWkend = 80607,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20080531,
                AccAltKeyNum = 154
            },

            new CaldarRecord
            {
                AccWkendN = 80607,
                AccApWkend = 80614,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20080607,
                AccAltKeyNum = 155
            },

            new CaldarRecord
            {
                AccWkendN = 80614,
                AccApWkend = 80621,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20080614,
                AccAltKeyNum = 156
            },

            new CaldarRecord
            {
                AccWkendN = 80621,
                AccApWkend = 80628,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20080621,
                AccAltKeyNum = 157
            },

            new CaldarRecord
            {
                AccWkendN = 80628,
                AccApWkend = 80705,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20080628,
                AccAltKeyNum = 158
            },

            new CaldarRecord
            {
                AccWkendN = 80705,
                AccApWkend = 80712,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20080705,
                AccAltKeyNum = 159
            },

            new CaldarRecord
            {
                AccWkendN = 80712,
                AccApWkend = 80719,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20080712,
                AccAltKeyNum = 160
            },

            new CaldarRecord
            {
                AccWkendN = 80719,
                AccApWkend = 80726,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20080719,
                AccAltKeyNum = 161
            },

            new CaldarRecord
            {
                AccWkendN = 80726,
                AccApWkend = 80802,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20080726,
                AccAltKeyNum = 162
            },

            new CaldarRecord
            {
                AccWkendN = 80802,
                AccApWkend = 80809,
                AccWeekN = 31,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 31,
                AccCln60Period = 7,
                AccCln61Week = 31,
                AccCln61Period = 7,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 7,
                AccWkend2N = 20080802,
                AccAltKeyNum = 163
            },

            new CaldarRecord
            {
                AccWkendN = 80809,
                AccApWkend = 80816,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20080809,
                AccAltKeyNum = 164
            },

            new CaldarRecord
            {
                AccWkendN = 80816,
                AccApWkend = 80823,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20080816,
                AccAltKeyNum = 165
            },

            new CaldarRecord
            {
                AccWkendN = 80823,
                AccApWkend = 80830,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20080823,
                AccAltKeyNum = 166
            },

            new CaldarRecord
            {
                AccWkendN = 80830,
                AccApWkend = 80906,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20080830,
                AccAltKeyNum = 167
            },

            new CaldarRecord
            {
                AccWkendN = 80906,
                AccApWkend = 80913,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20080906,
                AccAltKeyNum = 168
            },

            new CaldarRecord
            {
                AccWkendN = 80913,
                AccApWkend = 80920,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20080913,
                AccAltKeyNum = 169
            },

            new CaldarRecord
            {
                AccWkendN = 80920,
                AccApWkend = 80927,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20080920,
                AccAltKeyNum = 170
            },

            new CaldarRecord
            {
                AccWkendN = 80927,
                AccApWkend = 81004,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20080927,
                AccAltKeyNum = 171
            },

            new CaldarRecord
            {
                AccWkendN = 81004,
                AccApWkend = 81011,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20081004,
                AccAltKeyNum = 172
            },

            new CaldarRecord
            {
                AccWkendN = 81011,
                AccApWkend = 81018,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20081011,
                AccAltKeyNum = 173
            },

            new CaldarRecord
            {
                AccWkendN = 81018,
                AccApWkend = 81025,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20081018,
                AccAltKeyNum = 174
            },

            new CaldarRecord
            {
                AccWkendN = 81025,
                AccApWkend = 81101,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20081025,
                AccAltKeyNum = 175
            },

            new CaldarRecord
            {
                AccWkendN = 81101,
                AccApWkend = 81108,
                AccWeekN = 44,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 44,
                AccCln60Period = 10,
                AccCln61Week = 44,
                AccCln61Period = 10,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 10,
                AccWkend2N = 20081101,
                AccAltKeyNum = 176
            },

            new CaldarRecord
            {
                AccWkendN = 81108,
                AccApWkend = 81115,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20081108,
                AccAltKeyNum = 177
            },

            new CaldarRecord
            {
                AccWkendN = 81115,
                AccApWkend = 81122,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20081115,
                AccAltKeyNum = 178
            },

            new CaldarRecord
            {
                AccWkendN = 81122,
                AccApWkend = 81129,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20081122,
                AccAltKeyNum = 179
            },

            new CaldarRecord
            {
                AccWkendN = 81129,
                AccApWkend = 81206,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20081129,
                AccAltKeyNum = 180
            },

            new CaldarRecord
            {
                AccWkendN = 81206,
                AccApWkend = 81213,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20081206,
                AccAltKeyNum = 181
            },

            new CaldarRecord
            {
                AccWkendN = 81213,
                AccApWkend = 81220,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20081213,
                AccAltKeyNum = 182
            },

            new CaldarRecord
            {
                AccWkendN = 81220,
                AccApWkend = 81227,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20081220,
                AccAltKeyNum = 183
            },

            new CaldarRecord
            {
                AccWkendN = 81227,
                AccApWkend = 90103,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20081227,
                AccAltKeyNum = 184
            },

            new CaldarRecord
            {
                AccWkendN = 90103,
                AccApWkend = 90110,
                AccWeekN = 53,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 53,
                AccCln60Period = 12,
                AccCln61Week = 53,
                AccCln61Period = 12,
                AccCln7XWeek = 53,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 53,
                AccCln6XPeriod = 12,
                AccWkend2N = 20090103,
                AccAltKeyNum = 185
            },

            new CaldarRecord
            {
                AccWkendN = 90110,
                AccApWkend = 90117,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20090110,
                AccAltKeyNum = 193
            },

            new CaldarRecord
            {
                AccWkendN = 90117,
                AccApWkend = 90124,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20090117,
                AccAltKeyNum = 194
            },

            new CaldarRecord
            {
                AccWkendN = 90124,
                AccApWkend = 90131,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20090124,
                AccAltKeyNum = 195
            },

            new CaldarRecord
            {
                AccWkendN = 90131,
                AccApWkend = 90207,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20090131,
                AccAltKeyNum = 196
            },

            new CaldarRecord
            {
                AccWkendN = 90207,
                AccApWkend = 90214,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20090207,
                AccAltKeyNum = 197
            },

            new CaldarRecord
            {
                AccWkendN = 90214,
                AccApWkend = 90221,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20090214,
                AccAltKeyNum = 198
            },

            new CaldarRecord
            {
                AccWkendN = 90221,
                AccApWkend = 90228,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20090221,
                AccAltKeyNum = 199
            },

            new CaldarRecord
            {
                AccWkendN = 90228,
                AccApWkend = 90307,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20090228,
                AccAltKeyNum = 200
            },

            new CaldarRecord
            {
                AccWkendN = 90307,
                AccApWkend = 90314,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 20090307,
                AccAltKeyNum = 201
            },

            new CaldarRecord
            {
                AccWkendN = 90314,
                AccApWkend = 90321,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20090314,
                AccAltKeyNum = 202
            },

            new CaldarRecord
            {
                AccWkendN = 90321,
                AccApWkend = 90328,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20090321,
                AccAltKeyNum = 203
            },

            new CaldarRecord
            {
                AccWkendN = 90328,
                AccApWkend = 90404,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20090328,
                AccAltKeyNum = 204
            },

            new CaldarRecord
            {
                AccWkendN = 90404,
                AccApWkend = 90411,
                AccWeekN = 13,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 13,
                AccCln60Period = 4,
                AccCln61Week = 13,
                AccCln61Period = 4,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 4,
                AccWkend2N = 20090404,
                AccAltKeyNum = 205
            },

            new CaldarRecord
            {
                AccWkendN = 90411,
                AccApWkend = 90418,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20090411,
                AccAltKeyNum = 206
            },

            new CaldarRecord
            {
                AccWkendN = 90418,
                AccApWkend = 90425,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20090418,
                AccAltKeyNum = 207
            },

            new CaldarRecord
            {
                AccWkendN = 90425,
                AccApWkend = 90502,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20090425,
                AccAltKeyNum = 208
            },

            new CaldarRecord
            {
                AccWkendN = 90502,
                AccApWkend = 90509,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20090502,
                AccAltKeyNum = 209
            },

            new CaldarRecord
            {
                AccWkendN = 90509,
                AccApWkend = 90516,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20090509,
                AccAltKeyNum = 210
            },

            new CaldarRecord
            {
                AccWkendN = 90516,
                AccApWkend = 90523,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20090516,
                AccAltKeyNum = 211
            },

            new CaldarRecord
            {
                AccWkendN = 90523,
                AccApWkend = 90530,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20090523,
                AccAltKeyNum = 212
            },

            new CaldarRecord
            {
                AccWkendN = 90530,
                AccApWkend = 90606,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20090530,
                AccAltKeyNum = 213
            },

            new CaldarRecord
            {
                AccWkendN = 90606,
                AccApWkend = 90613,
                AccWeekN = 22,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 22,
                AccCln60Period = 6,
                AccCln61Week = 22,
                AccCln61Period = 6,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 6,
                AccWkend2N = 20090606,
                AccAltKeyNum = 214
            },

            new CaldarRecord
            {
                AccWkendN = 90613,
                AccApWkend = 90620,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20090613,
                AccAltKeyNum = 215
            },

            new CaldarRecord
            {
                AccWkendN = 90620,
                AccApWkend = 90627,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20090620,
                AccAltKeyNum = 216
            },

            new CaldarRecord
            {
                AccWkendN = 90627,
                AccApWkend = 90704,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20090627,
                AccAltKeyNum = 217
            },

            new CaldarRecord
            {
                AccWkendN = 90704,
                AccApWkend = 90711,
                AccWeekN = 26,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 26,
                AccCln60Period = 7,
                AccCln61Week = 26,
                AccCln61Period = 7,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 7,
                AccWkend2N = 20090704,
                AccAltKeyNum = 218
            },

            new CaldarRecord
            {
                AccWkendN = 90711,
                AccApWkend = 90718,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20090711,
                AccAltKeyNum = 219
            },

            new CaldarRecord
            {
                AccWkendN = 90718,
                AccApWkend = 90725,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20090718,
                AccAltKeyNum = 220
            },

            new CaldarRecord
            {
                AccWkendN = 90725,
                AccApWkend = 90801,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20090725,
                AccAltKeyNum = 221
            },

            new CaldarRecord
            {
                AccWkendN = 90801,
                AccApWkend = 90808,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20090801,
                AccAltKeyNum = 222
            },

            new CaldarRecord
            {
                AccWkendN = 90808,
                AccApWkend = 90815,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20090808,
                AccAltKeyNum = 223
            },

            new CaldarRecord
            {
                AccWkendN = 90815,
                AccApWkend = 90822,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20090815,
                AccAltKeyNum = 224
            },

            new CaldarRecord
            {
                AccWkendN = 90822,
                AccApWkend = 90829,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20090822,
                AccAltKeyNum = 225
            },

            new CaldarRecord
            {
                AccWkendN = 90829,
                AccApWkend = 90905,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20090829,
                AccAltKeyNum = 226
            },

            new CaldarRecord
            {
                AccWkendN = 90905,
                AccApWkend = 90912,
                AccWeekN = 35,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 35,
                AccCln60Period = 9,
                AccCln61Week = 35,
                AccCln61Period = 9,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 9,
                AccWkend2N = 20090905,
                AccAltKeyNum = 227
            },

            new CaldarRecord
            {
                AccWkendN = 90912,
                AccApWkend = 90919,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20090912,
                AccAltKeyNum = 228
            },

            new CaldarRecord
            {
                AccWkendN = 90919,
                AccApWkend = 90926,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20090919,
                AccAltKeyNum = 229
            },

            new CaldarRecord
            {
                AccWkendN = 90926,
                AccApWkend = 91003,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20090926,
                AccAltKeyNum = 230
            },

            new CaldarRecord
            {
                AccWkendN = 91003,
                AccApWkend = 91010,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20091003,
                AccAltKeyNum = 231
            },

            new CaldarRecord
            {
                AccWkendN = 91010,
                AccApWkend = 91017,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20091010,
                AccAltKeyNum = 232
            },

            new CaldarRecord
            {
                AccWkendN = 91017,
                AccApWkend = 91024,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20091017,
                AccAltKeyNum = 233
            },

            new CaldarRecord
            {
                AccWkendN = 91024,
                AccApWkend = 91031,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20091024,
                AccAltKeyNum = 234
            },

            new CaldarRecord
            {
                AccWkendN = 91031,
                AccApWkend = 91107,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20091031,
                AccAltKeyNum = 235
            },

            new CaldarRecord
            {
                AccWkendN = 91107,
                AccApWkend = 91114,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 20091107,
                AccAltKeyNum = 236
            },

            new CaldarRecord
            {
                AccWkendN = 91114,
                AccApWkend = 91121,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20091114,
                AccAltKeyNum = 237
            },

            new CaldarRecord
            {
                AccWkendN = 91121,
                AccApWkend = 91128,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20091121,
                AccAltKeyNum = 238
            },

            new CaldarRecord
            {
                AccWkendN = 91128,
                AccApWkend = 91205,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20091128,
                AccAltKeyNum = 239
            },

            new CaldarRecord
            {
                AccWkendN = 91205,
                AccApWkend = 91212,
                AccWeekN = 48,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 48,
                AccCln60Period = 12,
                AccCln61Week = 48,
                AccCln61Period = 12,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 12,
                AccWkend2N = 20091205,
                AccAltKeyNum = 240
            },

            new CaldarRecord
            {
                AccWkendN = 91212,
                AccApWkend = 91219,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20091212,
                AccAltKeyNum = 241
            },

            new CaldarRecord
            {
                AccWkendN = 91219,
                AccApWkend = 91226,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20091219,
                AccAltKeyNum = 242
            },

            new CaldarRecord
            {
                AccWkendN = 91226,
                AccApWkend = 100102,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20091226,
                AccAltKeyNum = 243
            },

            new CaldarRecord
            {
                AccWkendN = 30104,
                AccApWkend = 30111,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20030104,
                AccAltKeyNum = -167
            },

            new CaldarRecord
            {
                AccWkendN = 30118,
                AccApWkend = 30125,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20030118,
                AccAltKeyNum = -165
            },

            new CaldarRecord
            {
                AccWkendN = 30208,
                AccApWkend = 30215,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20030208,
                AccAltKeyNum = -162
            },

            new CaldarRecord
            {
                AccWkendN = 30215,
                AccApWkend = 30222,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20030215,
                AccAltKeyNum = -161
            },

            new CaldarRecord
            {
                AccWkendN = 30222,
                AccApWkend = 30301,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20030222,
                AccAltKeyNum = -160
            },

            new CaldarRecord
            {
                AccWkendN = 30315,
                AccApWkend = 30322,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20030315,
                AccAltKeyNum = -157
            },

            new CaldarRecord
            {
                AccWkendN = 30426,
                AccApWkend = 30503,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20030426,
                AccAltKeyNum = -151
            },

            new CaldarRecord
            {
                AccWkendN = 30531,
                AccApWkend = 30607,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20030531,
                AccAltKeyNum = -146
            },

            new CaldarRecord
            {
                AccWkendN = 50528,
                AccApWkend = 50604,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20050528,
                AccAltKeyNum = -27
            },

            new CaldarRecord
            {
                AccWkendN = 50702,
                AccApWkend = 50709,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20050702,
                AccAltKeyNum = -22
            },

            new CaldarRecord
            {
                AccWkendN = 50709,
                AccApWkend = 50716,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20050709,
                AccAltKeyNum = -21
            },

            new CaldarRecord
            {
                AccWkendN = 50806,
                AccApWkend = 50813,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20050806,
                AccAltKeyNum = -17
            },

            new CaldarRecord
            {
                AccWkendN = 50917,
                AccApWkend = 50924,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20050917,
                AccAltKeyNum = -11
            },

            new CaldarRecord
            {
                AccWkendN = 51008,
                AccApWkend = 51015,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20051008,
                AccAltKeyNum = -8
            },

            new CaldarRecord
            {
                AccWkendN = 51029,
                AccApWkend = 51105,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20051029,
                AccAltKeyNum = -5
            },

            new CaldarRecord
            {
                AccWkendN = 51126,
                AccApWkend = 51203,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20051126,
                AccAltKeyNum = -1
            },

            new CaldarRecord
            {
                AccWkendN = 100102,
                AccApWkend = 100109,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20100102,
                AccAltKeyNum = 244
            },

            new CaldarRecord
            {
                AccWkendN = 100109,
                AccApWkend = 100116,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20100109,
                AccAltKeyNum = 253
            },

            new CaldarRecord
            {
                AccWkendN = 100116,
                AccApWkend = 100123,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20100116,
                AccAltKeyNum = 254
            },

            new CaldarRecord
            {
                AccWkendN = 100123,
                AccApWkend = 100130,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20100123,
                AccAltKeyNum = 255
            },

            new CaldarRecord
            {
                AccWkendN = 100130,
                AccApWkend = 100206,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20100130,
                AccAltKeyNum = 256
            },

            new CaldarRecord
            {
                AccWkendN = 100206,
                AccApWkend = 100213,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20100206,
                AccAltKeyNum = 257
            },

            new CaldarRecord
            {
                AccWkendN = 100213,
                AccApWkend = 100220,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20100213,
                AccAltKeyNum = 258
            },

            new CaldarRecord
            {
                AccWkendN = 100220,
                AccApWkend = 100227,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20100220,
                AccAltKeyNum = 259
            },

            new CaldarRecord
            {
                AccWkendN = 100227,
                AccApWkend = 100306,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20100227,
                AccAltKeyNum = 260
            },

            new CaldarRecord
            {
                AccWkendN = 100306,
                AccApWkend = 100313,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 20100306,
                AccAltKeyNum = 261
            },

            new CaldarRecord
            {
                AccWkendN = 100313,
                AccApWkend = 100320,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20100313,
                AccAltKeyNum = 262
            },

            new CaldarRecord
            {
                AccWkendN = 100320,
                AccApWkend = 100327,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20100320,
                AccAltKeyNum = 263
            },

            new CaldarRecord
            {
                AccWkendN = 100327,
                AccApWkend = 100403,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20100327,
                AccAltKeyNum = 264
            },

            new CaldarRecord
            {
                AccWkendN = 100403,
                AccApWkend = 100410,
                AccWeekN = 13,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 13,
                AccCln60Period = 4,
                AccCln61Week = 13,
                AccCln61Period = 4,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 4,
                AccWkend2N = 20100403,
                AccAltKeyNum = 265
            },

            new CaldarRecord
            {
                AccWkendN = 100410,
                AccApWkend = 100417,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20100410,
                AccAltKeyNum = 266
            },

            new CaldarRecord
            {
                AccWkendN = 100417,
                AccApWkend = 100424,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20100417,
                AccAltKeyNum = 267
            },

            new CaldarRecord
            {
                AccWkendN = 100424,
                AccApWkend = 100501,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20100424,
                AccAltKeyNum = 268
            },

            new CaldarRecord
            {
                AccWkendN = 100501,
                AccApWkend = 100508,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20100501,
                AccAltKeyNum = 269
            },

            new CaldarRecord
            {
                AccWkendN = 100508,
                AccApWkend = 100515,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20100508,
                AccAltKeyNum = 270
            },

            new CaldarRecord
            {
                AccWkendN = 100515,
                AccApWkend = 100522,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20100515,
                AccAltKeyNum = 271
            },

            new CaldarRecord
            {
                AccWkendN = 100522,
                AccApWkend = 100529,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20100522,
                AccAltKeyNum = 272
            },

            new CaldarRecord
            {
                AccWkendN = 100529,
                AccApWkend = 100605,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20100529,
                AccAltKeyNum = 273
            },

            new CaldarRecord
            {
                AccWkendN = 100605,
                AccApWkend = 100612,
                AccWeekN = 22,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 22,
                AccCln60Period = 6,
                AccCln61Week = 22,
                AccCln61Period = 6,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 6,
                AccWkend2N = 20100605,
                AccAltKeyNum = 274
            },

            new CaldarRecord
            {
                AccWkendN = 100612,
                AccApWkend = 100619,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20100612,
                AccAltKeyNum = 275
            },

            new CaldarRecord
            {
                AccWkendN = 100619,
                AccApWkend = 100626,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20100619,
                AccAltKeyNum = 276
            },

            new CaldarRecord
            {
                AccWkendN = 100626,
                AccApWkend = 100703,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20100626,
                AccAltKeyNum = 277
            },

            new CaldarRecord
            {
                AccWkendN = 100703,
                AccApWkend = 100710,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20100703,
                AccAltKeyNum = 278
            },

            new CaldarRecord
            {
                AccWkendN = 100710,
                AccApWkend = 100717,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20100710,
                AccAltKeyNum = 279
            },

            new CaldarRecord
            {
                AccWkendN = 100717,
                AccApWkend = 100724,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20100717,
                AccAltKeyNum = 280
            },

            new CaldarRecord
            {
                AccWkendN = 100724,
                AccApWkend = 100731,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20100724,
                AccAltKeyNum = 281
            },

            new CaldarRecord
            {
                AccWkendN = 100731,
                AccApWkend = 100807,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20100731,
                AccAltKeyNum = 282
            },

            new CaldarRecord
            {
                AccWkendN = 100807,
                AccApWkend = 100814,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20100807,
                AccAltKeyNum = 283
            },

            new CaldarRecord
            {
                AccWkendN = 100814,
                AccApWkend = 100821,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20100814,
                AccAltKeyNum = 284
            },

            new CaldarRecord
            {
                AccWkendN = 100821,
                AccApWkend = 100828,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20100821,
                AccAltKeyNum = 285
            },

            new CaldarRecord
            {
                AccWkendN = 100828,
                AccApWkend = 100904,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20100828,
                AccAltKeyNum = 286
            },

            new CaldarRecord
            {
                AccWkendN = 100904,
                AccApWkend = 100911,
                AccWeekN = 35,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 35,
                AccCln60Period = 9,
                AccCln61Week = 35,
                AccCln61Period = 9,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 9,
                AccWkend2N = 20100904,
                AccAltKeyNum = 287
            },

            new CaldarRecord
            {
                AccWkendN = 100911,
                AccApWkend = 100918,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20100911,
                AccAltKeyNum = 288
            },

            new CaldarRecord
            {
                AccWkendN = 100918,
                AccApWkend = 100925,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20100918,
                AccAltKeyNum = 289
            },

            new CaldarRecord
            {
                AccWkendN = 100925,
                AccApWkend = 101002,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20100925,
                AccAltKeyNum = 290
            },

            new CaldarRecord
            {
                AccWkendN = 101002,
                AccApWkend = 101009,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20101002,
                AccAltKeyNum = 291
            },

            new CaldarRecord
            {
                AccWkendN = 101009,
                AccApWkend = 101016,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20101009,
                AccAltKeyNum = 292
            },

            new CaldarRecord
            {
                AccWkendN = 101016,
                AccApWkend = 101023,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20101016,
                AccAltKeyNum = 293
            },

            new CaldarRecord
            {
                AccWkendN = 101023,
                AccApWkend = 101030,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20101023,
                AccAltKeyNum = 294
            },

            new CaldarRecord
            {
                AccWkendN = 101030,
                AccApWkend = 101106,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20101030,
                AccAltKeyNum = 295
            },

            new CaldarRecord
            {
                AccWkendN = 101106,
                AccApWkend = 101113,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 20101106,
                AccAltKeyNum = 296
            },

            new CaldarRecord
            {
                AccWkendN = 101113,
                AccApWkend = 101120,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20101113,
                AccAltKeyNum = 297
            },

            new CaldarRecord
            {
                AccWkendN = 101120,
                AccApWkend = 101127,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20101120,
                AccAltKeyNum = 298
            },

            new CaldarRecord
            {
                AccWkendN = 101127,
                AccApWkend = 101204,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20101127,
                AccAltKeyNum = 299
            },

            new CaldarRecord
            {
                AccWkendN = 101204,
                AccApWkend = 101211,
                AccWeekN = 48,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 48,
                AccCln60Period = 12,
                AccCln61Week = 48,
                AccCln61Period = 12,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 12,
                AccWkend2N = 20101204,
                AccAltKeyNum = 300
            },

            new CaldarRecord
            {
                AccWkendN = 101211,
                AccApWkend = 101218,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20101211,
                AccAltKeyNum = 301
            },

            new CaldarRecord
            {
                AccWkendN = 101218,
                AccApWkend = 101225,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20101218,
                AccAltKeyNum = 302
            },

            new CaldarRecord
            {
                AccWkendN = 101225,
                AccApWkend = 110101,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20101225,
                AccAltKeyNum = 303
            },

            new CaldarRecord
            {
                AccWkendN = 110101,
                AccApWkend = 110108,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20110101,
                AccAltKeyNum = 304
            },

            new CaldarRecord
            {
                AccWkendN = 110108,
                AccApWkend = 110115,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20110108,
                AccAltKeyNum = 313
            },

            new CaldarRecord
            {
                AccWkendN = 110115,
                AccApWkend = 110122,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20110115,
                AccAltKeyNum = 314
            },

            new CaldarRecord
            {
                AccWkendN = 110122,
                AccApWkend = 110129,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20110122,
                AccAltKeyNum = 315
            },

            new CaldarRecord
            {
                AccWkendN = 110129,
                AccApWkend = 110205,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20110129,
                AccAltKeyNum = 316
            },

            new CaldarRecord
            {
                AccWkendN = 110205,
                AccApWkend = 110212,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20110205,
                AccAltKeyNum = 317
            },

            new CaldarRecord
            {
                AccWkendN = 110212,
                AccApWkend = 110219,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20110212,
                AccAltKeyNum = 318
            },

            new CaldarRecord
            {
                AccWkendN = 110219,
                AccApWkend = 110226,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20110219,
                AccAltKeyNum = 319
            },

            new CaldarRecord
            {
                AccWkendN = 110226,
                AccApWkend = 110305,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20110226,
                AccAltKeyNum = 320
            },

            new CaldarRecord
            {
                AccWkendN = 110305,
                AccApWkend = 110312,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 20110305,
                AccAltKeyNum = 321
            },

            new CaldarRecord
            {
                AccWkendN = 110312,
                AccApWkend = 110319,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20110312,
                AccAltKeyNum = 322
            },

            new CaldarRecord
            {
                AccWkendN = 110319,
                AccApWkend = 110326,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20110319,
                AccAltKeyNum = 323
            },

            new CaldarRecord
            {
                AccWkendN = 110326,
                AccApWkend = 110402,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20110326,
                AccAltKeyNum = 324
            },

            new CaldarRecord
            {
                AccWkendN = 110402,
                AccApWkend = 110409,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20110402,
                AccAltKeyNum = 325
            },

            new CaldarRecord
            {
                AccWkendN = 110409,
                AccApWkend = 110416,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20110409,
                AccAltKeyNum = 326
            },

            new CaldarRecord
            {
                AccWkendN = 110416,
                AccApWkend = 110423,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20110416,
                AccAltKeyNum = 327
            },

            new CaldarRecord
            {
                AccWkendN = 110423,
                AccApWkend = 110430,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20110423,
                AccAltKeyNum = 328
            },

            new CaldarRecord
            {
                AccWkendN = 110430,
                AccApWkend = 110507,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20110430,
                AccAltKeyNum = 329
            },

            new CaldarRecord
            {
                AccWkendN = 110507,
                AccApWkend = 110514,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20110507,
                AccAltKeyNum = 330
            },

            new CaldarRecord
            {
                AccWkendN = 110514,
                AccApWkend = 110521,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20110514,
                AccAltKeyNum = 331
            },

            new CaldarRecord
            {
                AccWkendN = 110521,
                AccApWkend = 110528,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20110521,
                AccAltKeyNum = 332
            },

            new CaldarRecord
            {
                AccWkendN = 110528,
                AccApWkend = 110604,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20110528,
                AccAltKeyNum = 333
            },

            new CaldarRecord
            {
                AccWkendN = 110604,
                AccApWkend = 110611,
                AccWeekN = 22,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 22,
                AccCln60Period = 6,
                AccCln61Week = 22,
                AccCln61Period = 6,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 6,
                AccWkend2N = 20110604,
                AccAltKeyNum = 334
            },

            new CaldarRecord
            {
                AccWkendN = 110611,
                AccApWkend = 110618,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20110611,
                AccAltKeyNum = 335
            },

            new CaldarRecord
            {
                AccWkendN = 110618,
                AccApWkend = 110625,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20110618,
                AccAltKeyNum = 336
            },

            new CaldarRecord
            {
                AccWkendN = 110625,
                AccApWkend = 110702,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20110625,
                AccAltKeyNum = 337
            },

            new CaldarRecord
            {
                AccWkendN = 110702,
                AccApWkend = 110709,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20110702,
                AccAltKeyNum = 338
            },

            new CaldarRecord
            {
                AccWkendN = 110709,
                AccApWkend = 110716,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20110709,
                AccAltKeyNum = 339
            },

            new CaldarRecord
            {
                AccWkendN = 110716,
                AccApWkend = 110723,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20110716,
                AccAltKeyNum = 340
            },

            new CaldarRecord
            {
                AccWkendN = 110723,
                AccApWkend = 110730,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20110723,
                AccAltKeyNum = 341
            },

            new CaldarRecord
            {
                AccWkendN = 110730,
                AccApWkend = 110806,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20110730,
                AccAltKeyNum = 342
            },

            new CaldarRecord
            {
                AccWkendN = 110806,
                AccApWkend = 110813,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20110806,
                AccAltKeyNum = 343
            },

            new CaldarRecord
            {
                AccWkendN = 110813,
                AccApWkend = 110820,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20110813,
                AccAltKeyNum = 344
            },

            new CaldarRecord
            {
                AccWkendN = 110820,
                AccApWkend = 110827,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20110820,
                AccAltKeyNum = 345
            },

            new CaldarRecord
            {
                AccWkendN = 110827,
                AccApWkend = 110903,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20110827,
                AccAltKeyNum = 346
            },

            new CaldarRecord
            {
                AccWkendN = 110903,
                AccApWkend = 110910,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20110903,
                AccAltKeyNum = 347
            },

            new CaldarRecord
            {
                AccWkendN = 110910,
                AccApWkend = 110917,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20110910,
                AccAltKeyNum = 348
            },

            new CaldarRecord
            {
                AccWkendN = 110917,
                AccApWkend = 110924,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20110917,
                AccAltKeyNum = 349
            },

            new CaldarRecord
            {
                AccWkendN = 110924,
                AccApWkend = 111001,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20110924,
                AccAltKeyNum = 350
            },

            new CaldarRecord
            {
                AccWkendN = 111001,
                AccApWkend = 111008,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20111001,
                AccAltKeyNum = 351
            },

            new CaldarRecord
            {
                AccWkendN = 111008,
                AccApWkend = 111015,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20111008,
                AccAltKeyNum = 352
            },

            new CaldarRecord
            {
                AccWkendN = 111015,
                AccApWkend = 111022,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20111015,
                AccAltKeyNum = 353
            },

            new CaldarRecord
            {
                AccWkendN = 111022,
                AccApWkend = 111029,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20111022,
                AccAltKeyNum = 354
            },

            new CaldarRecord
            {
                AccWkendN = 111029,
                AccApWkend = 111105,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20111029,
                AccAltKeyNum = 355
            },

            new CaldarRecord
            {
                AccWkendN = 111105,
                AccApWkend = 111112,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 20111105,
                AccAltKeyNum = 356
            },

            new CaldarRecord
            {
                AccWkendN = 111112,
                AccApWkend = 111119,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20111112,
                AccAltKeyNum = 357
            },

            new CaldarRecord
            {
                AccWkendN = 111119,
                AccApWkend = 111126,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20111119,
                AccAltKeyNum = 358
            },

            new CaldarRecord
            {
                AccWkendN = 111126,
                AccApWkend = 111203,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20111126,
                AccAltKeyNum = 359
            },

            new CaldarRecord
            {
                AccWkendN = 111203,
                AccApWkend = 111210,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20111203,
                AccAltKeyNum = 360
            },

            new CaldarRecord
            {
                AccWkendN = 111210,
                AccApWkend = 111217,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20111210,
                AccAltKeyNum = 361
            },

            new CaldarRecord
            {
                AccWkendN = 111217,
                AccApWkend = 111224,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20111217,
                AccAltKeyNum = 362
            },

            new CaldarRecord
            {
                AccWkendN = 111224,
                AccApWkend = 111231,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20111224,
                AccAltKeyNum = 363
            },

            new CaldarRecord
            {
                AccWkendN = 111231,
                AccApWkend = 120107,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20111231,
                AccAltKeyNum = 364
            },

            new CaldarRecord
            {
                AccWkendN = 120107,
                AccApWkend = 120114,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20120107,
                AccAltKeyNum = 373
            },

            new CaldarRecord
            {
                AccWkendN = 120114,
                AccApWkend = 120121,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20120114,
                AccAltKeyNum = 374
            },

            new CaldarRecord
            {
                AccWkendN = 120121,
                AccApWkend = 120128,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20120121,
                AccAltKeyNum = 375
            },

            new CaldarRecord
            {
                AccWkendN = 120128,
                AccApWkend = 120204,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20120128,
                AccAltKeyNum = 376
            },

            new CaldarRecord
            {
                AccWkendN = 120204,
                AccApWkend = 120211,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20120204,
                AccAltKeyNum = 377
            },

            new CaldarRecord
            {
                AccWkendN = 120211,
                AccApWkend = 120218,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20120211,
                AccAltKeyNum = 378
            },

            new CaldarRecord
            {
                AccWkendN = 120218,
                AccApWkend = 120225,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20120218,
                AccAltKeyNum = 379
            },

            new CaldarRecord
            {
                AccWkendN = 120225,
                AccApWkend = 120303,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20120225,
                AccAltKeyNum = 380
            },

            new CaldarRecord
            {
                AccWkendN = 120303,
                AccApWkend = 120310,
                AccWeekN = 9,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 9,
                AccCln60Period = 2,
                AccCln61Week = 9,
                AccCln61Period = 2,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 2,
                AccWkend2N = 20120303,
                AccAltKeyNum = 381
            },

            new CaldarRecord
            {
                AccWkendN = 120310,
                AccApWkend = 120317,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20120310,
                AccAltKeyNum = 382
            },

            new CaldarRecord
            {
                AccWkendN = 120317,
                AccApWkend = 120324,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20120317,
                AccAltKeyNum = 383
            },

            new CaldarRecord
            {
                AccWkendN = 120324,
                AccApWkend = 120331,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20120324,
                AccAltKeyNum = 384
            },

            new CaldarRecord
            {
                AccWkendN = 120331,
                AccApWkend = 120407,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20120331,
                AccAltKeyNum = 385
            },

            new CaldarRecord
            {
                AccWkendN = 120407,
                AccApWkend = 120414,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20120407,
                AccAltKeyNum = 386
            },

            new CaldarRecord
            {
                AccWkendN = 120414,
                AccApWkend = 120421,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20120414,
                AccAltKeyNum = 387
            },

            new CaldarRecord
            {
                AccWkendN = 120421,
                AccApWkend = 120428,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20120421,
                AccAltKeyNum = 388
            },

            new CaldarRecord
            {
                AccWkendN = 120428,
                AccApWkend = 120505,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20120428,
                AccAltKeyNum = 389
            },

            new CaldarRecord
            {
                AccWkendN = 120505,
                AccApWkend = 120512,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20120505,
                AccAltKeyNum = 390
            },

            new CaldarRecord
            {
                AccWkendN = 120512,
                AccApWkend = 120519,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20120512,
                AccAltKeyNum = 391
            },

            new CaldarRecord
            {
                AccWkendN = 120519,
                AccApWkend = 120526,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20120519,
                AccAltKeyNum = 392
            },

            new CaldarRecord
            {
                AccWkendN = 120526,
                AccApWkend = 120602,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20120526,
                AccAltKeyNum = 393
            },

            new CaldarRecord
            {
                AccWkendN = 120602,
                AccApWkend = 120609,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20120602,
                AccAltKeyNum = 394
            },

            new CaldarRecord
            {
                AccWkendN = 120609,
                AccApWkend = 120616,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20120609,
                AccAltKeyNum = 395
            },

            new CaldarRecord
            {
                AccWkendN = 120616,
                AccApWkend = 120623,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20120616,
                AccAltKeyNum = 396
            },

            new CaldarRecord
            {
                AccWkendN = 120623,
                AccApWkend = 120630,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20120623,
                AccAltKeyNum = 397
            },

            new CaldarRecord
            {
                AccWkendN = 120630,
                AccApWkend = 120707,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20120630,
                AccAltKeyNum = 398
            },

            new CaldarRecord
            {
                AccWkendN = 120707,
                AccApWkend = 120714,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20120707,
                AccAltKeyNum = 399
            },

            new CaldarRecord
            {
                AccWkendN = 120714,
                AccApWkend = 120721,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20120714,
                AccAltKeyNum = 400
            },

            new CaldarRecord
            {
                AccWkendN = 120721,
                AccApWkend = 120728,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20120721,
                AccAltKeyNum = 401
            },

            new CaldarRecord
            {
                AccWkendN = 120728,
                AccApWkend = 120804,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20120728,
                AccAltKeyNum = 402
            },

            new CaldarRecord
            {
                AccWkendN = 120804,
                AccApWkend = 120811,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20120804,
                AccAltKeyNum = 403
            },

            new CaldarRecord
            {
                AccWkendN = 120811,
                AccApWkend = 120818,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20120811,
                AccAltKeyNum = 404
            },

            new CaldarRecord
            {
                AccWkendN = 120818,
                AccApWkend = 120825,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20120818,
                AccAltKeyNum = 405
            },

            new CaldarRecord
            {
                AccWkendN = 120825,
                AccApWkend = 120901,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20120825,
                AccAltKeyNum = 406
            },

            new CaldarRecord
            {
                AccWkendN = 120901,
                AccApWkend = 120908,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20120901,
                AccAltKeyNum = 407
            },

            new CaldarRecord
            {
                AccWkendN = 120908,
                AccApWkend = 120915,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20120908,
                AccAltKeyNum = 408
            },

            new CaldarRecord
            {
                AccWkendN = 120915,
                AccApWkend = 120922,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20120915,
                AccAltKeyNum = 409
            },

            new CaldarRecord
            {
                AccWkendN = 120922,
                AccApWkend = 120929,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20120922,
                AccAltKeyNum = 410
            },

            new CaldarRecord
            {
                AccWkendN = 120929,
                AccApWkend = 121006,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20120929,
                AccAltKeyNum = 411
            },

            new CaldarRecord
            {
                AccWkendN = 121006,
                AccApWkend = 121013,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20121006,
                AccAltKeyNum = 412
            },

            new CaldarRecord
            {
                AccWkendN = 121013,
                AccApWkend = 121020,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20121013,
                AccAltKeyNum = 413
            },

            new CaldarRecord
            {
                AccWkendN = 121020,
                AccApWkend = 121027,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20121020,
                AccAltKeyNum = 414
            },

            new CaldarRecord
            {
                AccWkendN = 121027,
                AccApWkend = 121103,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20121027,
                AccAltKeyNum = 415
            },

            new CaldarRecord
            {
                AccWkendN = 121103,
                AccApWkend = 121110,
                AccWeekN = 44,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 44,
                AccCln60Period = 10,
                AccCln61Week = 44,
                AccCln61Period = 10,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 10,
                AccWkend2N = 20121103,
                AccAltKeyNum = 416
            },

            new CaldarRecord
            {
                AccWkendN = 121110,
                AccApWkend = 121117,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20121110,
                AccAltKeyNum = 417
            },

            new CaldarRecord
            {
                AccWkendN = 121117,
                AccApWkend = 121124,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20121117,
                AccAltKeyNum = 418
            },

            new CaldarRecord
            {
                AccWkendN = 121124,
                AccApWkend = 121201,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20121124,
                AccAltKeyNum = 419
            },

            new CaldarRecord
            {
                AccWkendN = 121201,
                AccApWkend = 121208,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20121201,
                AccAltKeyNum = 420
            },

            new CaldarRecord
            {
                AccWkendN = 121208,
                AccApWkend = 121215,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20121208,
                AccAltKeyNum = 421
            },

            new CaldarRecord
            {
                AccWkendN = 121215,
                AccApWkend = 121222,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20121215,
                AccAltKeyNum = 422
            },

            new CaldarRecord
            {
                AccWkendN = 121222,
                AccApWkend = 121229,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20121222,
                AccAltKeyNum = 423
            },

            new CaldarRecord
            {
                AccWkendN = 121229,
                AccApWkend = 130105,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20121229,
                AccAltKeyNum = 424
            },

            new CaldarRecord
            {
                AccWkendN = 130105,
                AccApWkend = 130112,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20130105,
                AccAltKeyNum = 433
            },

            new CaldarRecord
            {
                AccWkendN = 130112,
                AccApWkend = 130119,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20130112,
                AccAltKeyNum = 434
            },

            new CaldarRecord
            {
                AccWkendN = 130119,
                AccApWkend = 130126,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20130119,
                AccAltKeyNum = 435
            },

            new CaldarRecord
            {
                AccWkendN = 130126,
                AccApWkend = 130202,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20130126,
                AccAltKeyNum = 436
            },

            new CaldarRecord
            {
                AccWkendN = 130202,
                AccApWkend = 130209,
                AccWeekN = 5,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 5,
                AccCln60Period = 1,
                AccCln61Week = 5,
                AccCln61Period = 1,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 1,
                AccWkend2N = 20130202,
                AccAltKeyNum = 437
            },

            new CaldarRecord
            {
                AccWkendN = 130209,
                AccApWkend = 130216,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20130209,
                AccAltKeyNum = 438
            },

            new CaldarRecord
            {
                AccWkendN = 130216,
                AccApWkend = 130223,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20130216,
                AccAltKeyNum = 439
            },

            new CaldarRecord
            {
                AccWkendN = 130223,
                AccApWkend = 130302,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20130223,
                AccAltKeyNum = 440
            },

            new CaldarRecord
            {
                AccWkendN = 130302,
                AccApWkend = 130309,
                AccWeekN = 9,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 9,
                AccCln60Period = 2,
                AccCln61Week = 9,
                AccCln61Period = 2,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 2,
                AccWkend2N = 20130302,
                AccAltKeyNum = 441
            },

            new CaldarRecord
            {
                AccWkendN = 130309,
                AccApWkend = 130316,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20130309,
                AccAltKeyNum = 442
            },

            new CaldarRecord
            {
                AccWkendN = 130316,
                AccApWkend = 130323,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20130316,
                AccAltKeyNum = 443
            },

            new CaldarRecord
            {
                AccWkendN = 130323,
                AccApWkend = 130330,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20130323,
                AccAltKeyNum = 444
            },

            new CaldarRecord
            {
                AccWkendN = 130330,
                AccApWkend = 130406,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20130330,
                AccAltKeyNum = 445
            },

            new CaldarRecord
            {
                AccWkendN = 130406,
                AccApWkend = 130413,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20130406,
                AccAltKeyNum = 446
            },

            new CaldarRecord
            {
                AccWkendN = 130413,
                AccApWkend = 130420,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20130413,
                AccAltKeyNum = 447
            },

            new CaldarRecord
            {
                AccWkendN = 130420,
                AccApWkend = 130427,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20130420,
                AccAltKeyNum = 448
            },

            new CaldarRecord
            {
                AccWkendN = 130427,
                AccApWkend = 130504,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20130427,
                AccAltKeyNum = 449
            },

            new CaldarRecord
            {
                AccWkendN = 130504,
                AccApWkend = 130511,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20130504,
                AccAltKeyNum = 450
            },

            new CaldarRecord
            {
                AccWkendN = 130511,
                AccApWkend = 130518,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20130511,
                AccAltKeyNum = 451
            },

            new CaldarRecord
            {
                AccWkendN = 130518,
                AccApWkend = 130525,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20130518,
                AccAltKeyNum = 452
            },

            new CaldarRecord
            {
                AccWkendN = 130525,
                AccApWkend = 130601,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20130525,
                AccAltKeyNum = 453
            },

            new CaldarRecord
            {
                AccWkendN = 130601,
                AccApWkend = 130608,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20130601,
                AccAltKeyNum = 454
            },

            new CaldarRecord
            {
                AccWkendN = 130608,
                AccApWkend = 130615,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20130608,
                AccAltKeyNum = 455
            },

            new CaldarRecord
            {
                AccWkendN = 130615,
                AccApWkend = 130622,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20130615,
                AccAltKeyNum = 456
            },

            new CaldarRecord
            {
                AccWkendN = 130622,
                AccApWkend = 130629,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20130622,
                AccAltKeyNum = 457
            },

            new CaldarRecord
            {
                AccWkendN = 130629,
                AccApWkend = 130706,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20130629,
                AccAltKeyNum = 458
            },

            new CaldarRecord
            {
                AccWkendN = 130706,
                AccApWkend = 130713,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20130706,
                AccAltKeyNum = 459
            },

            new CaldarRecord
            {
                AccWkendN = 130713,
                AccApWkend = 130720,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20130713,
                AccAltKeyNum = 460
            },

            new CaldarRecord
            {
                AccWkendN = 130720,
                AccApWkend = 130727,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20130720,
                AccAltKeyNum = 461
            },

            new CaldarRecord
            {
                AccWkendN = 130727,
                AccApWkend = 130803,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20130727,
                AccAltKeyNum = 462
            },

            new CaldarRecord
            {
                AccWkendN = 130803,
                AccApWkend = 130810,
                AccWeekN = 31,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 31,
                AccCln60Period = 7,
                AccCln61Week = 31,
                AccCln61Period = 7,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 7,
                AccWkend2N = 20130803,
                AccAltKeyNum = 463
            },

            new CaldarRecord
            {
                AccWkendN = 130810,
                AccApWkend = 130817,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20130810,
                AccAltKeyNum = 464
            },

            new CaldarRecord
            {
                AccWkendN = 130817,
                AccApWkend = 130824,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20130817,
                AccAltKeyNum = 465
            },

            new CaldarRecord
            {
                AccWkendN = 130824,
                AccApWkend = 130831,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20130824,
                AccAltKeyNum = 466
            },

            new CaldarRecord
            {
                AccWkendN = 130831,
                AccApWkend = 130907,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20130831,
                AccAltKeyNum = 467
            },

            new CaldarRecord
            {
                AccWkendN = 130907,
                AccApWkend = 130914,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20130907,
                AccAltKeyNum = 468
            },

            new CaldarRecord
            {
                AccWkendN = 130914,
                AccApWkend = 130921,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20130914,
                AccAltKeyNum = 469
            },

            new CaldarRecord
            {
                AccWkendN = 130921,
                AccApWkend = 130928,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20130921,
                AccAltKeyNum = 470
            },

            new CaldarRecord
            {
                AccWkendN = 130928,
                AccApWkend = 131005,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20130928,
                AccAltKeyNum = 471
            },

            new CaldarRecord
            {
                AccWkendN = 131005,
                AccApWkend = 131012,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20131005,
                AccAltKeyNum = 472
            },

            new CaldarRecord
            {
                AccWkendN = 131012,
                AccApWkend = 131019,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20131012,
                AccAltKeyNum = 473
            },

            new CaldarRecord
            {
                AccWkendN = 131019,
                AccApWkend = 131026,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20131019,
                AccAltKeyNum = 474
            },

            new CaldarRecord
            {
                AccWkendN = 131026,
                AccApWkend = 131102,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20131026,
                AccAltKeyNum = 475
            },

            new CaldarRecord
            {
                AccWkendN = 131102,
                AccApWkend = 131109,
                AccWeekN = 44,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 44,
                AccCln60Period = 10,
                AccCln61Week = 44,
                AccCln61Period = 10,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 10,
                AccWkend2N = 20131102,
                AccAltKeyNum = 476
            },

            new CaldarRecord
            {
                AccWkendN = 131109,
                AccApWkend = 131116,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20131109,
                AccAltKeyNum = 477
            },

            new CaldarRecord
            {
                AccWkendN = 131116,
                AccApWkend = 131123,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20131116,
                AccAltKeyNum = 478
            },

            new CaldarRecord
            {
                AccWkendN = 131123,
                AccApWkend = 131130,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20131123,
                AccAltKeyNum = 479
            },

            new CaldarRecord
            {
                AccWkendN = 131130,
                AccApWkend = 131207,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20131130,
                AccAltKeyNum = 480
            },

            new CaldarRecord
            {
                AccWkendN = 131207,
                AccApWkend = 131214,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20131207,
                AccAltKeyNum = 481
            },

            new CaldarRecord
            {
                AccWkendN = 131214,
                AccApWkend = 131221,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20131214,
                AccAltKeyNum = 482
            },

            new CaldarRecord
            {
                AccWkendN = 131221,
                AccApWkend = 131228,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20131221,
                AccAltKeyNum = 483
            },

            new CaldarRecord
            {
                AccWkendN = 131228,
                AccApWkend = 140104,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20131228,
                AccAltKeyNum = 484
            },

            new CaldarRecord
            {
                AccWkendN = 140104,
                AccApWkend = 140111,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20140104,
                AccAltKeyNum = 493
            },

            new CaldarRecord
            {
                AccWkendN = 140111,
                AccApWkend = 140118,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20140111,
                AccAltKeyNum = 494
            },

            new CaldarRecord
            {
                AccWkendN = 140118,
                AccApWkend = 140125,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20140118,
                AccAltKeyNum = 495
            },

            new CaldarRecord
            {
                AccWkendN = 140125,
                AccApWkend = 140201,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20140125,
                AccAltKeyNum = 496
            },

            new CaldarRecord
            {
                AccWkendN = 140201,
                AccApWkend = 140208,
                AccWeekN = 5,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 5,
                AccCln60Period = 1,
                AccCln61Week = 5,
                AccCln61Period = 1,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 1,
                AccWkend2N = 20140201,
                AccAltKeyNum = 497
            },

            new CaldarRecord
            {
                AccWkendN = 140208,
                AccApWkend = 140215,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20140208,
                AccAltKeyNum = 498
            },

            new CaldarRecord
            {
                AccWkendN = 140215,
                AccApWkend = 140222,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20140215,
                AccAltKeyNum = 499
            },

            new CaldarRecord
            {
                AccWkendN = 140222,
                AccApWkend = 140301,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20140222,
                AccAltKeyNum = 500
            },

            new CaldarRecord
            {
                AccWkendN = 140301,
                AccApWkend = 140308,
                AccWeekN = 9,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 9,
                AccCln60Period = 2,
                AccCln61Week = 9,
                AccCln61Period = 2,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 2,
                AccWkend2N = 20140301,
                AccAltKeyNum = 501
            },

            new CaldarRecord
            {
                AccWkendN = 140308,
                AccApWkend = 140315,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20140308,
                AccAltKeyNum = 502
            },

            new CaldarRecord
            {
                AccWkendN = 140315,
                AccApWkend = 140322,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20140315,
                AccAltKeyNum = 503
            },

            new CaldarRecord
            {
                AccWkendN = 140322,
                AccApWkend = 140329,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20140322,
                AccAltKeyNum = 504
            },

            new CaldarRecord
            {
                AccWkendN = 140329,
                AccApWkend = 140405,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20140329,
                AccAltKeyNum = 505
            },

            new CaldarRecord
            {
                AccWkendN = 140405,
                AccApWkend = 140412,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20140405,
                AccAltKeyNum = 506
            },

            new CaldarRecord
            {
                AccWkendN = 140412,
                AccApWkend = 140419,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20140412,
                AccAltKeyNum = 507
            },

            new CaldarRecord
            {
                AccWkendN = 140419,
                AccApWkend = 140426,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20140419,
                AccAltKeyNum = 508
            },

            new CaldarRecord
            {
                AccWkendN = 140426,
                AccApWkend = 140503,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20140426,
                AccAltKeyNum = 509
            },

            new CaldarRecord
            {
                AccWkendN = 140503,
                AccApWkend = 140510,
                AccWeekN = 18,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 18,
                AccCln60Period = 4,
                AccCln61Week = 18,
                AccCln61Period = 4,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 4,
                AccWkend2N = 20140503,
                AccAltKeyNum = 510
            },

            new CaldarRecord
            {
                AccWkendN = 140510,
                AccApWkend = 140517,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20140510,
                AccAltKeyNum = 511
            },

            new CaldarRecord
            {
                AccWkendN = 140517,
                AccApWkend = 140524,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20140517,
                AccAltKeyNum = 512
            },

            new CaldarRecord
            {
                AccWkendN = 140524,
                AccApWkend = 140531,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20140524,
                AccAltKeyNum = 513
            },

            new CaldarRecord
            {
                AccWkendN = 140531,
                AccApWkend = 140607,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20140531,
                AccAltKeyNum = 514
            },

            new CaldarRecord
            {
                AccWkendN = 140607,
                AccApWkend = 140614,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20140607,
                AccAltKeyNum = 515
            },

            new CaldarRecord
            {
                AccWkendN = 140614,
                AccApWkend = 140621,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20140614,
                AccAltKeyNum = 516
            },

            new CaldarRecord
            {
                AccWkendN = 140621,
                AccApWkend = 140628,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20140621,
                AccAltKeyNum = 517
            },

            new CaldarRecord
            {
                AccWkendN = 140628,
                AccApWkend = 140705,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20140628,
                AccAltKeyNum = 518
            },

            new CaldarRecord
            {
                AccWkendN = 140705,
                AccApWkend = 140712,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20140705,
                AccAltKeyNum = 519
            },

            new CaldarRecord
            {
                AccWkendN = 140712,
                AccApWkend = 140719,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20140712,
                AccAltKeyNum = 520
            },

            new CaldarRecord
            {
                AccWkendN = 140719,
                AccApWkend = 140726,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20140719,
                AccAltKeyNum = 521
            },

            new CaldarRecord
            {
                AccWkendN = 140726,
                AccApWkend = 140802,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20140726,
                AccAltKeyNum = 522
            },

            new CaldarRecord
            {
                AccWkendN = 140802,
                AccApWkend = 140809,
                AccWeekN = 31,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 31,
                AccCln60Period = 7,
                AccCln61Week = 31,
                AccCln61Period = 7,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 7,
                AccWkend2N = 20140802,
                AccAltKeyNum = 523
            },

            new CaldarRecord
            {
                AccWkendN = 140809,
                AccApWkend = 140816,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20140809,
                AccAltKeyNum = 524
            },

            new CaldarRecord
            {
                AccWkendN = 140816,
                AccApWkend = 140823,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20140816,
                AccAltKeyNum = 525
            },

            new CaldarRecord
            {
                AccWkendN = 140823,
                AccApWkend = 140830,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20140823,
                AccAltKeyNum = 526
            },

            new CaldarRecord
            {
                AccWkendN = 140830,
                AccApWkend = 140906,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20140830,
                AccAltKeyNum = 527
            },

            new CaldarRecord
            {
                AccWkendN = 140906,
                AccApWkend = 140913,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20140906,
                AccAltKeyNum = 528
            },

            new CaldarRecord
            {
                AccWkendN = 140913,
                AccApWkend = 140920,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20140913,
                AccAltKeyNum = 529
            },

            new CaldarRecord
            {
                AccWkendN = 140920,
                AccApWkend = 140927,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20140920,
                AccAltKeyNum = 530
            },

            new CaldarRecord
            {
                AccWkendN = 140927,
                AccApWkend = 141004,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20140927,
                AccAltKeyNum = 531
            },

            new CaldarRecord
            {
                AccWkendN = 141004,
                AccApWkend = 141011,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20141004,
                AccAltKeyNum = 532
            },

            new CaldarRecord
            {
                AccWkendN = 141011,
                AccApWkend = 141018,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20141011,
                AccAltKeyNum = 533
            },

            new CaldarRecord
            {
                AccWkendN = 141018,
                AccApWkend = 141025,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20141018,
                AccAltKeyNum = 534
            },

            new CaldarRecord
            {
                AccWkendN = 141025,
                AccApWkend = 141101,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20141025,
                AccAltKeyNum = 535
            },

            new CaldarRecord
            {
                AccWkendN = 141101,
                AccApWkend = 141108,
                AccWeekN = 44,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 44,
                AccCln60Period = 10,
                AccCln61Week = 44,
                AccCln61Period = 10,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 10,
                AccWkend2N = 20141101,
                AccAltKeyNum = 536
            },

            new CaldarRecord
            {
                AccWkendN = 141108,
                AccApWkend = 141115,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20141108,
                AccAltKeyNum = 537
            },

            new CaldarRecord
            {
                AccWkendN = 141115,
                AccApWkend = 141122,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20141115,
                AccAltKeyNum = 538
            },

            new CaldarRecord
            {
                AccWkendN = 141122,
                AccApWkend = 141129,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20141122,
                AccAltKeyNum = 539
            },

            new CaldarRecord
            {
                AccWkendN = 141129,
                AccApWkend = 141206,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20141129,
                AccAltKeyNum = 540
            },

            new CaldarRecord
            {
                AccWkendN = 141206,
                AccApWkend = 141213,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20141206,
                AccAltKeyNum = 541
            },

            new CaldarRecord
            {
                AccWkendN = 141213,
                AccApWkend = 141220,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20141213,
                AccAltKeyNum = 542
            },

            new CaldarRecord
            {
                AccWkendN = 141220,
                AccApWkend = 141227,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20141220,
                AccAltKeyNum = 543
            },

            new CaldarRecord
            {
                AccWkendN = 141227,
                AccApWkend = 150103,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20141227,
                AccAltKeyNum = 544
            },

            new CaldarRecord
            {
                AccWkendN = 150103,
                AccApWkend = 150110,
                AccWeekN = 53,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 53,
                AccCln60Period = 12,
                AccCln61Week = 53,
                AccCln61Period = 12,
                AccCln7XWeek = 53,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 53,
                AccCln6XPeriod = 12,
                AccWkend2N = 20150103,
                AccAltKeyNum = 545
            },

            new CaldarRecord
            {
                AccWkendN = 150110,
                AccApWkend = 150117,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20150110,
                AccAltKeyNum = 553
            },

            new CaldarRecord
            {
                AccWkendN = 150117,
                AccApWkend = 150124,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20150117,
                AccAltKeyNum = 554
            },

            new CaldarRecord
            {
                AccWkendN = 150124,
                AccApWkend = 150131,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20150124,
                AccAltKeyNum = 555
            },

            new CaldarRecord
            {
                AccWkendN = 150131,
                AccApWkend = 150207,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20150131,
                AccAltKeyNum = 556
            },

            new CaldarRecord
            {
                AccWkendN = 150207,
                AccApWkend = 150214,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20150207,
                AccAltKeyNum = 557
            },

            new CaldarRecord
            {
                AccWkendN = 150214,
                AccApWkend = 150221,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20150214,
                AccAltKeyNum = 558
            },

            new CaldarRecord
            {
                AccWkendN = 150221,
                AccApWkend = 150228,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20150221,
                AccAltKeyNum = 559
            },

            new CaldarRecord
            {
                AccWkendN = 150228,
                AccApWkend = 150307,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20150228,
                AccAltKeyNum = 560
            },

            new CaldarRecord
            {
                AccWkendN = 150307,
                AccApWkend = 150314,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 20150307,
                AccAltKeyNum = 561
            },

            new CaldarRecord
            {
                AccWkendN = 150314,
                AccApWkend = 150321,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20150314,
                AccAltKeyNum = 562
            },

            new CaldarRecord
            {
                AccWkendN = 150321,
                AccApWkend = 150328,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20150321,
                AccAltKeyNum = 563
            },

            new CaldarRecord
            {
                AccWkendN = 150328,
                AccApWkend = 150404,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20150328,
                AccAltKeyNum = 564
            },

            new CaldarRecord
            {
                AccWkendN = 150404,
                AccApWkend = 150411,
                AccWeekN = 13,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 13,
                AccCln60Period = 4,
                AccCln61Week = 13,
                AccCln61Period = 4,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 4,
                AccWkend2N = 20150404,
                AccAltKeyNum = 565
            },

            new CaldarRecord
            {
                AccWkendN = 150411,
                AccApWkend = 150418,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20150411,
                AccAltKeyNum = 566
            },

            new CaldarRecord
            {
                AccWkendN = 150418,
                AccApWkend = 150425,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20150418,
                AccAltKeyNum = 567
            },

            new CaldarRecord
            {
                AccWkendN = 150425,
                AccApWkend = 150502,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20150425,
                AccAltKeyNum = 568
            },

            new CaldarRecord
            {
                AccWkendN = 150502,
                AccApWkend = 150509,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20150502,
                AccAltKeyNum = 569
            },

            new CaldarRecord
            {
                AccWkendN = 150509,
                AccApWkend = 150516,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20150509,
                AccAltKeyNum = 570
            },

            new CaldarRecord
            {
                AccWkendN = 150516,
                AccApWkend = 150523,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20150516,
                AccAltKeyNum = 571
            },

            new CaldarRecord
            {
                AccWkendN = 150523,
                AccApWkend = 150530,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20150523,
                AccAltKeyNum = 572
            },

            new CaldarRecord
            {
                AccWkendN = 150530,
                AccApWkend = 150606,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20150530,
                AccAltKeyNum = 573
            },

            new CaldarRecord
            {
                AccWkendN = 150606,
                AccApWkend = 150613,
                AccWeekN = 22,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 22,
                AccCln60Period = 6,
                AccCln61Week = 22,
                AccCln61Period = 6,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 6,
                AccWkend2N = 20150606,
                AccAltKeyNum = 574
            },

            new CaldarRecord
            {
                AccWkendN = 150613,
                AccApWkend = 150620,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20150613,
                AccAltKeyNum = 575
            },

            new CaldarRecord
            {
                AccWkendN = 150620,
                AccApWkend = 150627,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20150620,
                AccAltKeyNum = 576
            },

            new CaldarRecord
            {
                AccWkendN = 150627,
                AccApWkend = 150704,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20150627,
                AccAltKeyNum = 577
            },

            new CaldarRecord
            {
                AccWkendN = 150704,
                AccApWkend = 150711,
                AccWeekN = 26,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 26,
                AccCln60Period = 7,
                AccCln61Week = 26,
                AccCln61Period = 7,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 7,
                AccWkend2N = 20150704,
                AccAltKeyNum = 578
            },

            new CaldarRecord
            {
                AccWkendN = 150711,
                AccApWkend = 150718,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20150711,
                AccAltKeyNum = 579
            },

            new CaldarRecord
            {
                AccWkendN = 150718,
                AccApWkend = 150725,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20150718,
                AccAltKeyNum = 580
            },

            new CaldarRecord
            {
                AccWkendN = 150725,
                AccApWkend = 150801,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20150725,
                AccAltKeyNum = 581
            },

            new CaldarRecord
            {
                AccWkendN = 150801,
                AccApWkend = 150808,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20150801,
                AccAltKeyNum = 582
            },

            new CaldarRecord
            {
                AccWkendN = 150808,
                AccApWkend = 150815,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20150808,
                AccAltKeyNum = 583
            },

            new CaldarRecord
            {
                AccWkendN = 150815,
                AccApWkend = 150822,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20150815,
                AccAltKeyNum = 584
            },

            new CaldarRecord
            {
                AccWkendN = 150822,
                AccApWkend = 150829,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20150822,
                AccAltKeyNum = 585
            },

            new CaldarRecord
            {
                AccWkendN = 150829,
                AccApWkend = 150905,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20150829,
                AccAltKeyNum = 586
            },

            new CaldarRecord
            {
                AccWkendN = 150905,
                AccApWkend = 150912,
                AccWeekN = 35,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 35,
                AccCln60Period = 9,
                AccCln61Week = 35,
                AccCln61Period = 9,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 9,
                AccWkend2N = 20150905,
                AccAltKeyNum = 587
            },

            new CaldarRecord
            {
                AccWkendN = 150912,
                AccApWkend = 150919,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20150912,
                AccAltKeyNum = 588
            },

            new CaldarRecord
            {
                AccWkendN = 150919,
                AccApWkend = 150926,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20150919,
                AccAltKeyNum = 589
            },

            new CaldarRecord
            {
                AccWkendN = 150926,
                AccApWkend = 151003,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20150926,
                AccAltKeyNum = 590
            },

            new CaldarRecord
            {
                AccWkendN = 151003,
                AccApWkend = 151010,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20151003,
                AccAltKeyNum = 591
            },

            new CaldarRecord
            {
                AccWkendN = 151010,
                AccApWkend = 151017,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20151010,
                AccAltKeyNum = 592
            },

            new CaldarRecord
            {
                AccWkendN = 151017,
                AccApWkend = 151024,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20151017,
                AccAltKeyNum = 593
            },

            new CaldarRecord
            {
                AccWkendN = 151024,
                AccApWkend = 151031,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20151024,
                AccAltKeyNum = 594
            },

            new CaldarRecord
            {
                AccWkendN = 151031,
                AccApWkend = 151107,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20151031,
                AccAltKeyNum = 595
            },

            new CaldarRecord
            {
                AccWkendN = 151107,
                AccApWkend = 151114,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 20151107,
                AccAltKeyNum = 596
            },

            new CaldarRecord
            {
                AccWkendN = 151114,
                AccApWkend = 151121,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20151114,
                AccAltKeyNum = 597
            },

            new CaldarRecord
            {
                AccWkendN = 151121,
                AccApWkend = 151128,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20151121,
                AccAltKeyNum = 598
            },

            new CaldarRecord
            {
                AccWkendN = 151128,
                AccApWkend = 151205,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20151128,
                AccAltKeyNum = 599
            },

            new CaldarRecord
            {
                AccWkendN = 151205,
                AccApWkend = 151212,
                AccWeekN = 48,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 48,
                AccCln60Period = 12,
                AccCln61Week = 48,
                AccCln61Period = 12,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 12,
                AccWkend2N = 20151205,
                AccAltKeyNum = 600
            },

            new CaldarRecord
            {
                AccWkendN = 151212,
                AccApWkend = 151219,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20151212,
                AccAltKeyNum = 601
            },

            new CaldarRecord
            {
                AccWkendN = 151219,
                AccApWkend = 151226,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20151219,
                AccAltKeyNum = 602
            },

            new CaldarRecord
            {
                AccWkendN = 151226,
                AccApWkend = 160102,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20151226,
                AccAltKeyNum = 603
            },

            new CaldarRecord
            {
                AccWkendN = 160102,
                AccApWkend = 160109,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20160102,
                AccAltKeyNum = 604
            },

            new CaldarRecord
            {
                AccWkendN = 180106,
                AccApWkend = 180113,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20180106,
                AccAltKeyNum = 733
            },

            new CaldarRecord
            {
                AccWkendN = 180113,
                AccApWkend = 180120,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20180113,
                AccAltKeyNum = 734
            },

            new CaldarRecord
            {
                AccWkendN = 180120,
                AccApWkend = 180127,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20180120,
                AccAltKeyNum = 735
            },

            new CaldarRecord
            {
                AccWkendN = 180127,
                AccApWkend = 180203,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20180127,
                AccAltKeyNum = 736
            },

            new CaldarRecord
            {
                AccWkendN = 180203,
                AccApWkend = 180210,
                AccWeekN = 5,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 5,
                AccCln60Period = 1,
                AccCln61Week = 5,
                AccCln61Period = 1,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 1,
                AccWkend2N = 20180203,
                AccAltKeyNum = 737
            },

            new CaldarRecord
            {
                AccWkendN = 180210,
                AccApWkend = 180217,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20180210,
                AccAltKeyNum = 738
            },

            new CaldarRecord
            {
                AccWkendN = 180217,
                AccApWkend = 180224,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20180217,
                AccAltKeyNum = 739
            },

            new CaldarRecord
            {
                AccWkendN = 180224,
                AccApWkend = 180303,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20180224,
                AccAltKeyNum = 740
            },

            new CaldarRecord
            {
                AccWkendN = 180303,
                AccApWkend = 180310,
                AccWeekN = 9,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 9,
                AccCln60Period = 2,
                AccCln61Week = 9,
                AccCln61Period = 2,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 2,
                AccWkend2N = 20180303,
                AccAltKeyNum = 741
            },

            new CaldarRecord
            {
                AccWkendN = 180310,
                AccApWkend = 180317,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20180310,
                AccAltKeyNum = 742
            },

            new CaldarRecord
            {
                AccWkendN = 180317,
                AccApWkend = 180324,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20180317,
                AccAltKeyNum = 743
            },

            new CaldarRecord
            {
                AccWkendN = 180324,
                AccApWkend = 180331,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20180324,
                AccAltKeyNum = 744
            },

            new CaldarRecord
            {
                AccWkendN = 180331,
                AccApWkend = 180407,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20180331,
                AccAltKeyNum = 745
            },

            new CaldarRecord
            {
                AccWkendN = 180407,
                AccApWkend = 180414,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20180407,
                AccAltKeyNum = 746
            },

            new CaldarRecord
            {
                AccWkendN = 180414,
                AccApWkend = 180421,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20180414,
                AccAltKeyNum = 747
            },

            new CaldarRecord
            {
                AccWkendN = 180421,
                AccApWkend = 180428,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20180421,
                AccAltKeyNum = 748
            },

            new CaldarRecord
            {
                AccWkendN = 180428,
                AccApWkend = 180505,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20180428,
                AccAltKeyNum = 749
            },

            new CaldarRecord
            {
                AccWkendN = 180505,
                AccApWkend = 180512,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20180505,
                AccAltKeyNum = 750
            },

            new CaldarRecord
            {
                AccWkendN = 180512,
                AccApWkend = 180519,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20180512,
                AccAltKeyNum = 751
            },

            new CaldarRecord
            {
                AccWkendN = 180519,
                AccApWkend = 180526,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20180519,
                AccAltKeyNum = 752
            },

            new CaldarRecord
            {
                AccWkendN = 180526,
                AccApWkend = 180602,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20180526,
                AccAltKeyNum = 753
            },

            new CaldarRecord
            {
                AccWkendN = 180602,
                AccApWkend = 180609,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20180602,
                AccAltKeyNum = 754
            },

            new CaldarRecord
            {
                AccWkendN = 180609,
                AccApWkend = 180616,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20180609,
                AccAltKeyNum = 755
            },

            new CaldarRecord
            {
                AccWkendN = 180616,
                AccApWkend = 180623,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20180616,
                AccAltKeyNum = 756
            },

            new CaldarRecord
            {
                AccWkendN = 180623,
                AccApWkend = 180630,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20180623,
                AccAltKeyNum = 757
            },

            new CaldarRecord
            {
                AccWkendN = 180630,
                AccApWkend = 180707,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20180630,
                AccAltKeyNum = 758
            },

            new CaldarRecord
            {
                AccWkendN = 180707,
                AccApWkend = 180714,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20180707,
                AccAltKeyNum = 759
            },

            new CaldarRecord
            {
                AccWkendN = 180714,
                AccApWkend = 180721,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20180714,
                AccAltKeyNum = 760
            },

            new CaldarRecord
            {
                AccWkendN = 180721,
                AccApWkend = 180728,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20180721,
                AccAltKeyNum = 761
            },

            new CaldarRecord
            {
                AccWkendN = 180728,
                AccApWkend = 180804,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20180728,
                AccAltKeyNum = 762
            },

            new CaldarRecord
            {
                AccWkendN = 180804,
                AccApWkend = 180811,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20180804,
                AccAltKeyNum = 763
            },

            new CaldarRecord
            {
                AccWkendN = 180811,
                AccApWkend = 180818,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20180811,
                AccAltKeyNum = 764
            },

            new CaldarRecord
            {
                AccWkendN = 180818,
                AccApWkend = 180825,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20180818,
                AccAltKeyNum = 765
            },

            new CaldarRecord
            {
                AccWkendN = 180825,
                AccApWkend = 180901,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20180825,
                AccAltKeyNum = 766
            },

            new CaldarRecord
            {
                AccWkendN = 180901,
                AccApWkend = 180908,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20180901,
                AccAltKeyNum = 767
            },

            new CaldarRecord
            {
                AccWkendN = 180908,
                AccApWkend = 180915,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20180908,
                AccAltKeyNum = 768
            },

            new CaldarRecord
            {
                AccWkendN = 180915,
                AccApWkend = 180922,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20180915,
                AccAltKeyNum = 769
            },

            new CaldarRecord
            {
                AccWkendN = 180922,
                AccApWkend = 180929,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20180922,
                AccAltKeyNum = 770
            },

            new CaldarRecord
            {
                AccWkendN = 180929,
                AccApWkend = 181006,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20180929,
                AccAltKeyNum = 771
            },

            new CaldarRecord
            {
                AccWkendN = 181006,
                AccApWkend = 181013,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20181006,
                AccAltKeyNum = 772
            },

            new CaldarRecord
            {
                AccWkendN = 181013,
                AccApWkend = 181020,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20181013,
                AccAltKeyNum = 773
            },

            new CaldarRecord
            {
                AccWkendN = 181020,
                AccApWkend = 181027,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20181020,
                AccAltKeyNum = 774
            },

            new CaldarRecord
            {
                AccWkendN = 181027,
                AccApWkend = 181103,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20181027,
                AccAltKeyNum = 775
            },

            new CaldarRecord
            {
                AccWkendN = 181103,
                AccApWkend = 181110,
                AccWeekN = 44,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 44,
                AccCln60Period = 10,
                AccCln61Week = 44,
                AccCln61Period = 10,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 10,
                AccWkend2N = 20181103,
                AccAltKeyNum = 776
            },

            new CaldarRecord
            {
                AccWkendN = 181110,
                AccApWkend = 181117,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20181110,
                AccAltKeyNum = 777
            },

            new CaldarRecord
            {
                AccWkendN = 181117,
                AccApWkend = 181124,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20181117,
                AccAltKeyNum = 778
            },

            new CaldarRecord
            {
                AccWkendN = 181124,
                AccApWkend = 181201,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20181124,
                AccAltKeyNum = 779
            },

            new CaldarRecord
            {
                AccWkendN = 181201,
                AccApWkend = 181208,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20181201,
                AccAltKeyNum = 780
            },

            new CaldarRecord
            {
                AccWkendN = 181208,
                AccApWkend = 181215,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20181208,
                AccAltKeyNum = 781
            },

            new CaldarRecord
            {
                AccWkendN = 181215,
                AccApWkend = 181222,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20181215,
                AccAltKeyNum = 782
            },

            new CaldarRecord
            {
                AccWkendN = 181222,
                AccApWkend = 181229,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20181222,
                AccAltKeyNum = 783
            },

            new CaldarRecord
            {
                AccWkendN = 181229,
                AccApWkend = 190105,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20181229,
                AccAltKeyNum = 784
            },

            new CaldarRecord
            {
                AccWkendN = 190105,
                AccApWkend = 190112,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20190105,
                AccAltKeyNum = 793
            },

            new CaldarRecord
            {
                AccWkendN = 190112,
                AccApWkend = 190119,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20190112,
                AccAltKeyNum = 794
            },

            new CaldarRecord
            {
                AccWkendN = 190119,
                AccApWkend = 190126,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20190119,
                AccAltKeyNum = 795
            },

            new CaldarRecord
            {
                AccWkendN = 190126,
                AccApWkend = 190202,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20190126,
                AccAltKeyNum = 796
            },

            new CaldarRecord
            {
                AccWkendN = 190202,
                AccApWkend = 190209,
                AccWeekN = 5,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 5,
                AccCln60Period = 1,
                AccCln61Week = 5,
                AccCln61Period = 1,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 1,
                AccWkend2N = 20190202,
                AccAltKeyNum = 797
            },

            new CaldarRecord
            {
                AccWkendN = 190209,
                AccApWkend = 190216,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20190209,
                AccAltKeyNum = 798
            },

            new CaldarRecord
            {
                AccWkendN = 190216,
                AccApWkend = 190223,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20190216,
                AccAltKeyNum = 799
            },

            new CaldarRecord
            {
                AccWkendN = 190223,
                AccApWkend = 190302,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20190223,
                AccAltKeyNum = 800
            },

            new CaldarRecord
            {
                AccWkendN = 190302,
                AccApWkend = 190309,
                AccWeekN = 9,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 9,
                AccCln60Period = 2,
                AccCln61Week = 9,
                AccCln61Period = 2,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 2,
                AccWkend2N = 20190302,
                AccAltKeyNum = 801
            },

            new CaldarRecord
            {
                AccWkendN = 190309,
                AccApWkend = 190316,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20190309,
                AccAltKeyNum = 802
            },

            new CaldarRecord
            {
                AccWkendN = 190316,
                AccApWkend = 190323,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20190316,
                AccAltKeyNum = 803
            },

            new CaldarRecord
            {
                AccWkendN = 190323,
                AccApWkend = 190330,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20190323,
                AccAltKeyNum = 804
            },

            new CaldarRecord
            {
                AccWkendN = 190330,
                AccApWkend = 190406,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20190330,
                AccAltKeyNum = 805
            },

            new CaldarRecord
            {
                AccWkendN = 190406,
                AccApWkend = 190413,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20190406,
                AccAltKeyNum = 806
            },

            new CaldarRecord
            {
                AccWkendN = 190413,
                AccApWkend = 190420,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20190413,
                AccAltKeyNum = 807
            },

            new CaldarRecord
            {
                AccWkendN = 190420,
                AccApWkend = 190427,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20190420,
                AccAltKeyNum = 808
            },

            new CaldarRecord
            {
                AccWkendN = 190427,
                AccApWkend = 190504,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20190427,
                AccAltKeyNum = 809
            },

            new CaldarRecord
            {
                AccWkendN = 190504,
                AccApWkend = 190511,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20190504,
                AccAltKeyNum = 810
            },

            new CaldarRecord
            {
                AccWkendN = 190511,
                AccApWkend = 190518,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20190511,
                AccAltKeyNum = 811
            },

            new CaldarRecord
            {
                AccWkendN = 190518,
                AccApWkend = 190525,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20190518,
                AccAltKeyNum = 812
            },

            new CaldarRecord
            {
                AccWkendN = 190525,
                AccApWkend = 190601,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20190525,
                AccAltKeyNum = 813
            },

            new CaldarRecord
            {
                AccWkendN = 190601,
                AccApWkend = 190608,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20190601,
                AccAltKeyNum = 814
            },

            new CaldarRecord
            {
                AccWkendN = 190608,
                AccApWkend = 190615,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20190608,
                AccAltKeyNum = 815
            },

            new CaldarRecord
            {
                AccWkendN = 190615,
                AccApWkend = 190622,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20190615,
                AccAltKeyNum = 816
            },

            new CaldarRecord
            {
                AccWkendN = 190622,
                AccApWkend = 190629,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20190622,
                AccAltKeyNum = 817
            },

            new CaldarRecord
            {
                AccWkendN = 190629,
                AccApWkend = 190706,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20190629,
                AccAltKeyNum = 818
            },

            new CaldarRecord
            {
                AccWkendN = 190706,
                AccApWkend = 190713,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20190706,
                AccAltKeyNum = 819
            },

            new CaldarRecord
            {
                AccWkendN = 190713,
                AccApWkend = 190720,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20190713,
                AccAltKeyNum = 820
            },

            new CaldarRecord
            {
                AccWkendN = 190720,
                AccApWkend = 190727,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20190720,
                AccAltKeyNum = 821
            },

            new CaldarRecord
            {
                AccWkendN = 190727,
                AccApWkend = 190803,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20190727,
                AccAltKeyNum = 822
            },

            new CaldarRecord
            {
                AccWkendN = 190803,
                AccApWkend = 190810,
                AccWeekN = 31,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 31,
                AccCln60Period = 7,
                AccCln61Week = 31,
                AccCln61Period = 7,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 7,
                AccWkend2N = 20190803,
                AccAltKeyNum = 823
            },

            new CaldarRecord
            {
                AccWkendN = 190810,
                AccApWkend = 190817,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20190810,
                AccAltKeyNum = 824
            },

            new CaldarRecord
            {
                AccWkendN = 190817,
                AccApWkend = 190824,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20190817,
                AccAltKeyNum = 825
            },

            new CaldarRecord
            {
                AccWkendN = 190824,
                AccApWkend = 190831,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20190824,
                AccAltKeyNum = 826
            },

            new CaldarRecord
            {
                AccWkendN = 190831,
                AccApWkend = 190907,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20190831,
                AccAltKeyNum = 827
            },

            new CaldarRecord
            {
                AccWkendN = 190907,
                AccApWkend = 190914,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20190907,
                AccAltKeyNum = 828
            },

            new CaldarRecord
            {
                AccWkendN = 190914,
                AccApWkend = 190921,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20190914,
                AccAltKeyNum = 829
            },

            new CaldarRecord
            {
                AccWkendN = 190921,
                AccApWkend = 190928,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20190921,
                AccAltKeyNum = 830
            },

            new CaldarRecord
            {
                AccWkendN = 190928,
                AccApWkend = 191005,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20190928,
                AccAltKeyNum = 831
            },

            new CaldarRecord
            {
                AccWkendN = 191005,
                AccApWkend = 191012,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20191005,
                AccAltKeyNum = 832
            },

            new CaldarRecord
            {
                AccWkendN = 191012,
                AccApWkend = 191019,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20191012,
                AccAltKeyNum = 833
            },

            new CaldarRecord
            {
                AccWkendN = 191019,
                AccApWkend = 191026,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20191019,
                AccAltKeyNum = 834
            },

            new CaldarRecord
            {
                AccWkendN = 191026,
                AccApWkend = 191102,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20191026,
                AccAltKeyNum = 835
            },

            new CaldarRecord
            {
                AccWkendN = 191102,
                AccApWkend = 191109,
                AccWeekN = 44,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 44,
                AccCln60Period = 10,
                AccCln61Week = 44,
                AccCln61Period = 10,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 10,
                AccWkend2N = 20191102,
                AccAltKeyNum = 836
            },

            new CaldarRecord
            {
                AccWkendN = 191109,
                AccApWkend = 191116,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20191109,
                AccAltKeyNum = 837
            },

            new CaldarRecord
            {
                AccWkendN = 191116,
                AccApWkend = 191123,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20191116,
                AccAltKeyNum = 838
            },

            new CaldarRecord
            {
                AccWkendN = 191123,
                AccApWkend = 191130,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20191123,
                AccAltKeyNum = 839
            },

            new CaldarRecord
            {
                AccWkendN = 191130,
                AccApWkend = 191207,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20191130,
                AccAltKeyNum = 840
            },

            new CaldarRecord
            {
                AccWkendN = 191207,
                AccApWkend = 191214,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20191207,
                AccAltKeyNum = 841
            },

            new CaldarRecord
            {
                AccWkendN = 191214,
                AccApWkend = 191221,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20191214,
                AccAltKeyNum = 842
            },

            new CaldarRecord
            {
                AccWkendN = 191221,
                AccApWkend = 191228,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20191221,
                AccAltKeyNum = 843
            },

            new CaldarRecord
            {
                AccWkendN = 191228,
                AccApWkend = 200104,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20191228,
                AccAltKeyNum = 844
            },

            new CaldarRecord
            {
                AccWkendN = 200104,
                AccApWkend = 200111,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20200104,
                AccAltKeyNum = 853
            },

            new CaldarRecord
            {
                AccWkendN = 200111,
                AccApWkend = 200118,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20200111,
                AccAltKeyNum = 854
            },

            new CaldarRecord
            {
                AccWkendN = 200118,
                AccApWkend = 200125,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20200118,
                AccAltKeyNum = 855
            },

            new CaldarRecord
            {
                AccWkendN = 200125,
                AccApWkend = 200201,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20200125,
                AccAltKeyNum = 856
            },

            new CaldarRecord
            {
                AccWkendN = 200201,
                AccApWkend = 200208,
                AccWeekN = 5,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 5,
                AccCln60Period = 1,
                AccCln61Week = 5,
                AccCln61Period = 1,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 1,
                AccWkend2N = 20200201,
                AccAltKeyNum = 857
            },

            new CaldarRecord
            {
                AccWkendN = 200208,
                AccApWkend = 200215,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20200208,
                AccAltKeyNum = 858
            },

            new CaldarRecord
            {
                AccWkendN = 200215,
                AccApWkend = 200222,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20200215,
                AccAltKeyNum = 859
            },

            new CaldarRecord
            {
                AccWkendN = 200222,
                AccApWkend = 200229,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20200222,
                AccAltKeyNum = 860
            },

            new CaldarRecord
            {
                AccWkendN = 200229,
                AccApWkend = 200307,
                AccWeekN = 9,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 9,
                AccCln60Period = 2,
                AccCln61Week = 9,
                AccCln61Period = 2,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 2,
                AccWkend2N = 20200229,
                AccAltKeyNum = 861
            },

            new CaldarRecord
            {
                AccWkendN = 200307,
                AccApWkend = 200314,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20200307,
                AccAltKeyNum = 862
            },

            new CaldarRecord
            {
                AccWkendN = 200314,
                AccApWkend = 200321,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20200314,
                AccAltKeyNum = 863
            },

            new CaldarRecord
            {
                AccWkendN = 200321,
                AccApWkend = 200328,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20200321,
                AccAltKeyNum = 864
            },

            new CaldarRecord
            {
                AccWkendN = 200328,
                AccApWkend = 200404,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20200328,
                AccAltKeyNum = 865
            },

            new CaldarRecord
            {
                AccWkendN = 200404,
                AccApWkend = 200411,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20200404,
                AccAltKeyNum = 866
            },

            new CaldarRecord
            {
                AccWkendN = 200411,
                AccApWkend = 200418,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20200411,
                AccAltKeyNum = 867
            },

            new CaldarRecord
            {
                AccWkendN = 200418,
                AccApWkend = 200425,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20200418,
                AccAltKeyNum = 868
            },

            new CaldarRecord
            {
                AccWkendN = 200425,
                AccApWkend = 200502,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20200425,
                AccAltKeyNum = 869
            },

            new CaldarRecord
            {
                AccWkendN = 200502,
                AccApWkend = 200509,
                AccWeekN = 18,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 18,
                AccCln60Period = 4,
                AccCln61Week = 18,
                AccCln61Period = 4,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 4,
                AccWkend2N = 20200502,
                AccAltKeyNum = 870
            },

            new CaldarRecord
            {
                AccWkendN = 200509,
                AccApWkend = 200516,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20200509,
                AccAltKeyNum = 871
            },

            new CaldarRecord
            {
                AccWkendN = 200516,
                AccApWkend = 200523,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20200516,
                AccAltKeyNum = 872
            },

            new CaldarRecord
            {
                AccWkendN = 200523,
                AccApWkend = 200530,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20200523,
                AccAltKeyNum = 873
            },

            new CaldarRecord
            {
                AccWkendN = 200530,
                AccApWkend = 200606,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20200530,
                AccAltKeyNum = 874
            },

            new CaldarRecord
            {
                AccWkendN = 200606,
                AccApWkend = 200613,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20200606,
                AccAltKeyNum = 875
            },

            new CaldarRecord
            {
                AccWkendN = 200613,
                AccApWkend = 200620,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20200613,
                AccAltKeyNum = 876
            },

            new CaldarRecord
            {
                AccWkendN = 200620,
                AccApWkend = 200627,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20200620,
                AccAltKeyNum = 877
            },

            new CaldarRecord
            {
                AccWkendN = 200627,
                AccApWkend = 200704,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20200627,
                AccAltKeyNum = 878
            },

            new CaldarRecord
            {
                AccWkendN = 200704,
                AccApWkend = 200711,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20200704,
                AccAltKeyNum = 879
            },

            new CaldarRecord
            {
                AccWkendN = 200711,
                AccApWkend = 200718,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20200711,
                AccAltKeyNum = 880
            },

            new CaldarRecord
            {
                AccWkendN = 200718,
                AccApWkend = 200725,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20200718,
                AccAltKeyNum = 881
            },

            new CaldarRecord
            {
                AccWkendN = 200725,
                AccApWkend = 200801,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20200725,
                AccAltKeyNum = 882
            },

            new CaldarRecord
            {
                AccWkendN = 200801,
                AccApWkend = 200808,
                AccWeekN = 31,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 31,
                AccCln60Period = 7,
                AccCln61Week = 31,
                AccCln61Period = 7,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 7,
                AccWkend2N = 20200801,
                AccAltKeyNum = 883
            },

            new CaldarRecord
            {
                AccWkendN = 200808,
                AccApWkend = 200815,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20200808,
                AccAltKeyNum = 884
            },

            new CaldarRecord
            {
                AccWkendN = 200815,
                AccApWkend = 200822,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20200815,
                AccAltKeyNum = 885
            },

            new CaldarRecord
            {
                AccWkendN = 200822,
                AccApWkend = 200829,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20200822,
                AccAltKeyNum = 886
            },

            new CaldarRecord
            {
                AccWkendN = 200829,
                AccApWkend = 200905,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20200829,
                AccAltKeyNum = 887
            },

            new CaldarRecord
            {
                AccWkendN = 200905,
                AccApWkend = 200912,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20200905,
                AccAltKeyNum = 888
            },

            new CaldarRecord
            {
                AccWkendN = 200912,
                AccApWkend = 200919,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20200912,
                AccAltKeyNum = 889
            },

            new CaldarRecord
            {
                AccWkendN = 200919,
                AccApWkend = 200926,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20200919,
                AccAltKeyNum = 890
            },

            new CaldarRecord
            {
                AccWkendN = 200926,
                AccApWkend = 201003,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20200926,
                AccAltKeyNum = 891
            },

            new CaldarRecord
            {
                AccWkendN = 201003,
                AccApWkend = 201010,
                AccWeekN = 40,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 40,
                AccCln60Period = 9,
                AccCln61Week = 40,
                AccCln61Period = 9,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 9,
                AccWkend2N = 20201003,
                AccAltKeyNum = 892
            },

            new CaldarRecord
            {
                AccWkendN = 201010,
                AccApWkend = 201017,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20201010,
                AccAltKeyNum = 893
            },

            new CaldarRecord
            {
                AccWkendN = 201017,
                AccApWkend = 201024,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20201017,
                AccAltKeyNum = 894
            },

            new CaldarRecord
            {
                AccWkendN = 201024,
                AccApWkend = 201031,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20201024,
                AccAltKeyNum = 895
            },

            new CaldarRecord
            {
                AccWkendN = 201031,
                AccApWkend = 201107,
                AccWeekN = 44,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 44,
                AccCln60Period = 10,
                AccCln61Week = 44,
                AccCln61Period = 10,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 10,
                AccWkend2N = 20201031,
                AccAltKeyNum = 896
            },

            new CaldarRecord
            {
                AccWkendN = 201107,
                AccApWkend = 201114,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20201107,
                AccAltKeyNum = 897
            },

            new CaldarRecord
            {
                AccWkendN = 201114,
                AccApWkend = 201121,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20201114,
                AccAltKeyNum = 898
            },

            new CaldarRecord
            {
                AccWkendN = 201121,
                AccApWkend = 201128,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20201121,
                AccAltKeyNum = 899
            },

            new CaldarRecord
            {
                AccWkendN = 201128,
                AccApWkend = 201205,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20201128,
                AccAltKeyNum = 900
            },

            new CaldarRecord
            {
                AccWkendN = 201205,
                AccApWkend = 201212,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20201205,
                AccAltKeyNum = 901
            },

            new CaldarRecord
            {
                AccWkendN = 201212,
                AccApWkend = 201219,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20201212,
                AccAltKeyNum = 902
            },

            new CaldarRecord
            {
                AccWkendN = 201219,
                AccApWkend = 201226,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20201219,
                AccAltKeyNum = 903
            },

            new CaldarRecord
            {
                AccWkendN = 201226,
                AccApWkend = 210102,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20201226,
                AccAltKeyNum = 904
            },

            new CaldarRecord
            {
                AccWkendN = 210102,
                AccApWkend = 210109,
                AccWeekN = 53,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 53,
                AccCln60Period = 12,
                AccCln61Week = 53,
                AccCln61Period = 12,
                AccCln7XWeek = 53,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 53,
                AccCln6XPeriod = 12,
                AccWkend2N = 20210102,
                AccAltKeyNum = 905
            },

            new CaldarRecord
            {
                AccWkendN = 160109,
                AccApWkend = 160116,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20160109,
                AccAltKeyNum = 613
            },

            new CaldarRecord
            {
                AccWkendN = 160116,
                AccApWkend = 160123,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20160116,
                AccAltKeyNum = 614
            },

            new CaldarRecord
            {
                AccWkendN = 160123,
                AccApWkend = 160130,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20160123,
                AccAltKeyNum = 615
            },

            new CaldarRecord
            {
                AccWkendN = 160130,
                AccApWkend = 160206,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20160130,
                AccAltKeyNum = 616
            },

            new CaldarRecord
            {
                AccWkendN = 160206,
                AccApWkend = 160213,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20160206,
                AccAltKeyNum = 617
            },

            new CaldarRecord
            {
                AccWkendN = 160213,
                AccApWkend = 160220,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20160213,
                AccAltKeyNum = 618
            },

            new CaldarRecord
            {
                AccWkendN = 160220,
                AccApWkend = 160227,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20160220,
                AccAltKeyNum = 619
            },

            new CaldarRecord
            {
                AccWkendN = 160227,
                AccApWkend = 160305,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20160227,
                AccAltKeyNum = 620
            },

            new CaldarRecord
            {
                AccWkendN = 160305,
                AccApWkend = 160312,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 20160305,
                AccAltKeyNum = 621
            },

            new CaldarRecord
            {
                AccWkendN = 160312,
                AccApWkend = 160319,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20160312,
                AccAltKeyNum = 622
            },

            new CaldarRecord
            {
                AccWkendN = 160319,
                AccApWkend = 160326,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20160319,
                AccAltKeyNum = 623
            },

            new CaldarRecord
            {
                AccWkendN = 160326,
                AccApWkend = 160402,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20160326,
                AccAltKeyNum = 624
            },

            new CaldarRecord
            {
                AccWkendN = 160402,
                AccApWkend = 160409,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20160402,
                AccAltKeyNum = 625
            },

            new CaldarRecord
            {
                AccWkendN = 160409,
                AccApWkend = 160416,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20160409,
                AccAltKeyNum = 626
            },

            new CaldarRecord
            {
                AccWkendN = 160416,
                AccApWkend = 160423,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20160416,
                AccAltKeyNum = 627
            },

            new CaldarRecord
            {
                AccWkendN = 160423,
                AccApWkend = 160430,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20160423,
                AccAltKeyNum = 628
            },

            new CaldarRecord
            {
                AccWkendN = 160430,
                AccApWkend = 160507,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20160430,
                AccAltKeyNum = 629
            },

            new CaldarRecord
            {
                AccWkendN = 160507,
                AccApWkend = 160514,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20160507,
                AccAltKeyNum = 630
            },

            new CaldarRecord
            {
                AccWkendN = 160514,
                AccApWkend = 160521,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20160514,
                AccAltKeyNum = 631
            },

            new CaldarRecord
            {
                AccWkendN = 160521,
                AccApWkend = 160528,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20160521,
                AccAltKeyNum = 632
            },

            new CaldarRecord
            {
                AccWkendN = 160528,
                AccApWkend = 160604,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20160528,
                AccAltKeyNum = 633
            },

            new CaldarRecord
            {
                AccWkendN = 160604,
                AccApWkend = 160611,
                AccWeekN = 22,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 22,
                AccCln60Period = 6,
                AccCln61Week = 22,
                AccCln61Period = 6,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 6,
                AccWkend2N = 20160604,
                AccAltKeyNum = 634
            },

            new CaldarRecord
            {
                AccWkendN = 160611,
                AccApWkend = 160618,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20160611,
                AccAltKeyNum = 635
            },

            new CaldarRecord
            {
                AccWkendN = 160618,
                AccApWkend = 160625,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20160618,
                AccAltKeyNum = 636
            },

            new CaldarRecord
            {
                AccWkendN = 160625,
                AccApWkend = 160702,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20160625,
                AccAltKeyNum = 637
            },

            new CaldarRecord
            {
                AccWkendN = 160702,
                AccApWkend = 160709,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20160702,
                AccAltKeyNum = 638
            },

            new CaldarRecord
            {
                AccWkendN = 160709,
                AccApWkend = 160716,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20160709,
                AccAltKeyNum = 639
            },

            new CaldarRecord
            {
                AccWkendN = 160716,
                AccApWkend = 160723,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20160716,
                AccAltKeyNum = 640
            },

            new CaldarRecord
            {
                AccWkendN = 160723,
                AccApWkend = 160730,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20160723,
                AccAltKeyNum = 641
            },

            new CaldarRecord
            {
                AccWkendN = 160730,
                AccApWkend = 160806,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20160730,
                AccAltKeyNum = 642
            },

            new CaldarRecord
            {
                AccWkendN = 160806,
                AccApWkend = 160813,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20160806,
                AccAltKeyNum = 643
            },

            new CaldarRecord
            {
                AccWkendN = 160813,
                AccApWkend = 160820,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20160813,
                AccAltKeyNum = 644
            },

            new CaldarRecord
            {
                AccWkendN = 160820,
                AccApWkend = 160827,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20160820,
                AccAltKeyNum = 645
            },

            new CaldarRecord
            {
                AccWkendN = 160827,
                AccApWkend = 160903,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20160827,
                AccAltKeyNum = 646
            },

            new CaldarRecord
            {
                AccWkendN = 160903,
                AccApWkend = 160910,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20160903,
                AccAltKeyNum = 647
            },

            new CaldarRecord
            {
                AccWkendN = 160910,
                AccApWkend = 160917,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20160910,
                AccAltKeyNum = 648
            },

            new CaldarRecord
            {
                AccWkendN = 160917,
                AccApWkend = 160924,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20160917,
                AccAltKeyNum = 649
            },

            new CaldarRecord
            {
                AccWkendN = 160924,
                AccApWkend = 161001,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20160924,
                AccAltKeyNum = 650
            },

            new CaldarRecord
            {
                AccWkendN = 161001,
                AccApWkend = 161008,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20161001,
                AccAltKeyNum = 651
            },

            new CaldarRecord
            {
                AccWkendN = 161008,
                AccApWkend = 161015,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20161008,
                AccAltKeyNum = 652
            },

            new CaldarRecord
            {
                AccWkendN = 161015,
                AccApWkend = 161022,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20161015,
                AccAltKeyNum = 653
            },

            new CaldarRecord
            {
                AccWkendN = 161022,
                AccApWkend = 161029,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20161022,
                AccAltKeyNum = 654
            },

            new CaldarRecord
            {
                AccWkendN = 161029,
                AccApWkend = 161105,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20161029,
                AccAltKeyNum = 655
            },

            new CaldarRecord
            {
                AccWkendN = 161105,
                AccApWkend = 161112,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 20161105,
                AccAltKeyNum = 656
            },

            new CaldarRecord
            {
                AccWkendN = 161112,
                AccApWkend = 161119,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20161112,
                AccAltKeyNum = 657
            },

            new CaldarRecord
            {
                AccWkendN = 161119,
                AccApWkend = 161126,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20161119,
                AccAltKeyNum = 658
            },

            new CaldarRecord
            {
                AccWkendN = 161126,
                AccApWkend = 161203,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20161126,
                AccAltKeyNum = 659
            },

            new CaldarRecord
            {
                AccWkendN = 161203,
                AccApWkend = 161210,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20161203,
                AccAltKeyNum = 660
            },

            new CaldarRecord
            {
                AccWkendN = 161210,
                AccApWkend = 161217,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20161210,
                AccAltKeyNum = 661
            },

            new CaldarRecord
            {
                AccWkendN = 161217,
                AccApWkend = 161224,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20161217,
                AccAltKeyNum = 662
            },

            new CaldarRecord
            {
                AccWkendN = 161224,
                AccApWkend = 161231,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20161224,
                AccAltKeyNum = 663
            },

            new CaldarRecord
            {
                AccWkendN = 161231,
                AccApWkend = 170107,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20161231,
                AccAltKeyNum = 664
            },

            new CaldarRecord
            {
                AccWkendN = 170107,
                AccApWkend = 170114,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20170107,
                AccAltKeyNum = 673
            },

            new CaldarRecord
            {
                AccWkendN = 170114,
                AccApWkend = 170121,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20170114,
                AccAltKeyNum = 674
            },

            new CaldarRecord
            {
                AccWkendN = 170121,
                AccApWkend = 170128,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20170121,
                AccAltKeyNum = 675
            },

            new CaldarRecord
            {
                AccWkendN = 170128,
                AccApWkend = 170204,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20170128,
                AccAltKeyNum = 676
            },

            new CaldarRecord
            {
                AccWkendN = 170204,
                AccApWkend = 170211,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20170204,
                AccAltKeyNum = 677
            },

            new CaldarRecord
            {
                AccWkendN = 170211,
                AccApWkend = 170218,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20170211,
                AccAltKeyNum = 678
            },

            new CaldarRecord
            {
                AccWkendN = 170218,
                AccApWkend = 170225,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20170218,
                AccAltKeyNum = 679
            },

            new CaldarRecord
            {
                AccWkendN = 170225,
                AccApWkend = 170304,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20170225,
                AccAltKeyNum = 680
            },

            new CaldarRecord
            {
                AccWkendN = 170304,
                AccApWkend = 170311,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 20170304,
                AccAltKeyNum = 681
            },

            new CaldarRecord
            {
                AccWkendN = 170311,
                AccApWkend = 170318,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20170311,
                AccAltKeyNum = 682
            },

            new CaldarRecord
            {
                AccWkendN = 170318,
                AccApWkend = 170325,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20170318,
                AccAltKeyNum = 683
            },

            new CaldarRecord
            {
                AccWkendN = 170325,
                AccApWkend = 170401,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20170325,
                AccAltKeyNum = 684
            },

            new CaldarRecord
            {
                AccWkendN = 170401,
                AccApWkend = 170408,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20170401,
                AccAltKeyNum = 685
            },

            new CaldarRecord
            {
                AccWkendN = 170408,
                AccApWkend = 170415,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20170408,
                AccAltKeyNum = 686
            },

            new CaldarRecord
            {
                AccWkendN = 170415,
                AccApWkend = 170422,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20170415,
                AccAltKeyNum = 687
            },

            new CaldarRecord
            {
                AccWkendN = 170422,
                AccApWkend = 170429,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20170422,
                AccAltKeyNum = 688
            },

            new CaldarRecord
            {
                AccWkendN = 170429,
                AccApWkend = 170506,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20170429,
                AccAltKeyNum = 689
            },

            new CaldarRecord
            {
                AccWkendN = 170506,
                AccApWkend = 170513,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20170506,
                AccAltKeyNum = 690
            },

            new CaldarRecord
            {
                AccWkendN = 170513,
                AccApWkend = 170520,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20170513,
                AccAltKeyNum = 691
            },

            new CaldarRecord
            {
                AccWkendN = 170520,
                AccApWkend = 170527,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20170520,
                AccAltKeyNum = 692
            },

            new CaldarRecord
            {
                AccWkendN = 170527,
                AccApWkend = 170603,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20170527,
                AccAltKeyNum = 693
            },

            new CaldarRecord
            {
                AccWkendN = 170603,
                AccApWkend = 170610,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20170603,
                AccAltKeyNum = 694
            },

            new CaldarRecord
            {
                AccWkendN = 170610,
                AccApWkend = 170617,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20170610,
                AccAltKeyNum = 695
            },

            new CaldarRecord
            {
                AccWkendN = 170617,
                AccApWkend = 170624,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20170617,
                AccAltKeyNum = 696
            },

            new CaldarRecord
            {
                AccWkendN = 170624,
                AccApWkend = 170701,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20170624,
                AccAltKeyNum = 697
            },

            new CaldarRecord
            {
                AccWkendN = 170701,
                AccApWkend = 170708,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20170701,
                AccAltKeyNum = 698
            },

            new CaldarRecord
            {
                AccWkendN = 170708,
                AccApWkend = 170715,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20170708,
                AccAltKeyNum = 699
            },

            new CaldarRecord
            {
                AccWkendN = 170715,
                AccApWkend = 170722,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20170715,
                AccAltKeyNum = 700
            },

            new CaldarRecord
            {
                AccWkendN = 170722,
                AccApWkend = 170729,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20170722,
                AccAltKeyNum = 701
            },

            new CaldarRecord
            {
                AccWkendN = 170729,
                AccApWkend = 170805,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20170729,
                AccAltKeyNum = 702
            },

            new CaldarRecord
            {
                AccWkendN = 170805,
                AccApWkend = 170812,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20170805,
                AccAltKeyNum = 703
            },

            new CaldarRecord
            {
                AccWkendN = 170812,
                AccApWkend = 170819,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20170812,
                AccAltKeyNum = 704
            },

            new CaldarRecord
            {
                AccWkendN = 170819,
                AccApWkend = 170826,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20170819,
                AccAltKeyNum = 705
            },

            new CaldarRecord
            {
                AccWkendN = 170826,
                AccApWkend = 170902,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20170826,
                AccAltKeyNum = 706
            },

            new CaldarRecord
            {
                AccWkendN = 170902,
                AccApWkend = 170909,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20170902,
                AccAltKeyNum = 707
            },

            new CaldarRecord
            {
                AccWkendN = 170909,
                AccApWkend = 170916,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20170909,
                AccAltKeyNum = 708
            },

            new CaldarRecord
            {
                AccWkendN = 170916,
                AccApWkend = 170923,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20170916,
                AccAltKeyNum = 709
            },

            new CaldarRecord
            {
                AccWkendN = 170923,
                AccApWkend = 170930,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20170923,
                AccAltKeyNum = 710
            },

            new CaldarRecord
            {
                AccWkendN = 170930,
                AccApWkend = 171007,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20170930,
                AccAltKeyNum = 711
            },

            new CaldarRecord
            {
                AccWkendN = 171007,
                AccApWkend = 171014,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20171007,
                AccAltKeyNum = 712
            },

            new CaldarRecord
            {
                AccWkendN = 171014,
                AccApWkend = 171021,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20171014,
                AccAltKeyNum = 713
            },

            new CaldarRecord
            {
                AccWkendN = 171021,
                AccApWkend = 171028,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20171021,
                AccAltKeyNum = 714
            },

            new CaldarRecord
            {
                AccWkendN = 171028,
                AccApWkend = 171104,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20171028,
                AccAltKeyNum = 715
            },

            new CaldarRecord
            {
                AccWkendN = 171104,
                AccApWkend = 171111,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 20171104,
                AccAltKeyNum = 716
            },

            new CaldarRecord
            {
                AccWkendN = 171111,
                AccApWkend = 171118,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20171111,
                AccAltKeyNum = 717
            },

            new CaldarRecord
            {
                AccWkendN = 171118,
                AccApWkend = 171125,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20171118,
                AccAltKeyNum = 718
            },

            new CaldarRecord
            {
                AccWkendN = 171125,
                AccApWkend = 171202,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20171125,
                AccAltKeyNum = 719
            },

            new CaldarRecord
            {
                AccWkendN = 171202,
                AccApWkend = 171209,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20171202,
                AccAltKeyNum = 720
            },

            new CaldarRecord
            {
                AccWkendN = 171209,
                AccApWkend = 171216,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20171209,
                AccAltKeyNum = 721
            },

            new CaldarRecord
            {
                AccWkendN = 171216,
                AccApWkend = 171223,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20171216,
                AccAltKeyNum = 722
            },

            new CaldarRecord
            {
                AccWkendN = 171223,
                AccApWkend = 171230,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20171223,
                AccAltKeyNum = 723
            },

            new CaldarRecord
            {
                AccWkendN = 171230,
                AccApWkend = 180106,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20171230,
                AccAltKeyNum = 724
            },

            new CaldarRecord
            {
                AccWkendN = 10120,
                AccApWkend = 10127,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20010120,
                AccAltKeyNum = -285
            },

            new CaldarRecord
            {
                AccWkendN = 10127,
                AccApWkend = 10203,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20010127,
                AccAltKeyNum = -284
            },

            new CaldarRecord
            {
                AccWkendN = 10203,
                AccApWkend = 10210,
                AccWeekN = 5,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 5,
                AccCln60Period = 1,
                AccCln61Week = 5,
                AccCln61Period = 1,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 1,
                AccWkend2N = 20010203,
                AccAltKeyNum = -283
            },

            new CaldarRecord
            {
                AccWkendN = 10210,
                AccApWkend = 10217,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20010210,
                AccAltKeyNum = -282
            },

            new CaldarRecord
            {
                AccWkendN = 10224,
                AccApWkend = 10303,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20010224,
                AccAltKeyNum = -280
            },

            new CaldarRecord
            {
                AccWkendN = 10303,
                AccApWkend = 10310,
                AccWeekN = 9,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 9,
                AccCln60Period = 2,
                AccCln61Week = 9,
                AccCln61Period = 2,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 2,
                AccWkend2N = 20010303,
                AccAltKeyNum = -279
            },

            new CaldarRecord
            {
                AccWkendN = 10310,
                AccApWkend = 10317,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20010310,
                AccAltKeyNum = -278
            },

            new CaldarRecord
            {
                AccWkendN = 10317,
                AccApWkend = 10324,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20010317,
                AccAltKeyNum = -277
            },

            new CaldarRecord
            {
                AccWkendN = 10331,
                AccApWkend = 10407,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20010331,
                AccAltKeyNum = -275
            },

            new CaldarRecord
            {
                AccWkendN = 10407,
                AccApWkend = 10414,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20010407,
                AccAltKeyNum = -274
            },

            new CaldarRecord
            {
                AccWkendN = 10414,
                AccApWkend = 10421,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20010414,
                AccAltKeyNum = -273
            },

            new CaldarRecord
            {
                AccWkendN = 10421,
                AccApWkend = 10428,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20010421,
                AccAltKeyNum = -272
            },

            new CaldarRecord
            {
                AccWkendN = 10428,
                AccApWkend = 10505,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20010428,
                AccAltKeyNum = -271
            },

            new CaldarRecord
            {
                AccWkendN = 10505,
                AccApWkend = 10512,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20010505,
                AccAltKeyNum = -270
            },

            new CaldarRecord
            {
                AccWkendN = 10512,
                AccApWkend = 10519,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20010512,
                AccAltKeyNum = -269
            },

            new CaldarRecord
            {
                AccWkendN = 10519,
                AccApWkend = 10526,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20010519,
                AccAltKeyNum = -268
            },

            new CaldarRecord
            {
                AccWkendN = 10602,
                AccApWkend = 10609,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20010602,
                AccAltKeyNum = -266
            },

            new CaldarRecord
            {
                AccWkendN = 10609,
                AccApWkend = 10616,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20010609,
                AccAltKeyNum = -265
            },

            new CaldarRecord
            {
                AccWkendN = 10616,
                AccApWkend = 10623,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20010616,
                AccAltKeyNum = -264
            },

            new CaldarRecord
            {
                AccWkendN = 10630,
                AccApWkend = 10707,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20010630,
                AccAltKeyNum = -262
            },

            new CaldarRecord
            {
                AccWkendN = 10707,
                AccApWkend = 10714,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20010707,
                AccAltKeyNum = -261
            },

            new CaldarRecord
            {
                AccWkendN = 10714,
                AccApWkend = 10721,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20010714,
                AccAltKeyNum = -260
            },

            new CaldarRecord
            {
                AccWkendN = 10721,
                AccApWkend = 10728,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20010721,
                AccAltKeyNum = -259
            },

            new CaldarRecord
            {
                AccWkendN = 10728,
                AccApWkend = 10804,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20010728,
                AccAltKeyNum = -258
            },

            new CaldarRecord
            {
                AccWkendN = 10811,
                AccApWkend = 10818,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20010811,
                AccAltKeyNum = -256
            },

            new CaldarRecord
            {
                AccWkendN = 10818,
                AccApWkend = 10825,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20010818,
                AccAltKeyNum = -255
            },

            new CaldarRecord
            {
                AccWkendN = 10825,
                AccApWkend = 10901,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20010825,
                AccAltKeyNum = -254
            },

            new CaldarRecord
            {
                AccWkendN = 10901,
                AccApWkend = 10908,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20010901,
                AccAltKeyNum = -253
            },

            new CaldarRecord
            {
                AccWkendN = 10922,
                AccApWkend = 10929,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20010922,
                AccAltKeyNum = -250
            },

            new CaldarRecord
            {
                AccWkendN = 10929,
                AccApWkend = 11006,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20010929,
                AccAltKeyNum = -249
            },

            new CaldarRecord
            {
                AccWkendN = 11006,
                AccApWkend = 11013,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20011006,
                AccAltKeyNum = -248
            },

            new CaldarRecord
            {
                AccWkendN = 11013,
                AccApWkend = 11020,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20011013,
                AccAltKeyNum = -247
            },

            new CaldarRecord
            {
                AccWkendN = 11027,
                AccApWkend = 11103,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20011027,
                AccAltKeyNum = -245
            },

            new CaldarRecord
            {
                AccWkendN = 11110,
                AccApWkend = 11117,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20011110,
                AccAltKeyNum = -243
            },

            new CaldarRecord
            {
                AccWkendN = 11117,
                AccApWkend = 11124,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20011117,
                AccAltKeyNum = -242
            },

            new CaldarRecord
            {
                AccWkendN = 11201,
                AccApWkend = 11208,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20011201,
                AccAltKeyNum = -240
            },

            new CaldarRecord
            {
                AccWkendN = 11208,
                AccApWkend = 11215,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20011208,
                AccAltKeyNum = -239
            },

            new CaldarRecord
            {
                AccWkendN = 11222,
                AccApWkend = 11229,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20011222,
                AccAltKeyNum = -237
            },

            new CaldarRecord
            {
                AccWkendN = 11229,
                AccApWkend = 20105,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20011229,
                AccAltKeyNum = -236
            },

            new CaldarRecord
            {
                AccWkendN = 20105,
                AccApWkend = 20112,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20020105,
                AccAltKeyNum = -227
            },

            new CaldarRecord
            {
                AccWkendN = 20112,
                AccApWkend = 20119,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20020112,
                AccAltKeyNum = -226
            },

            new CaldarRecord
            {
                AccWkendN = 20119,
                AccApWkend = 20126,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20020119,
                AccAltKeyNum = -225
            },

            new CaldarRecord
            {
                AccWkendN = 20126,
                AccApWkend = 20202,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20020126,
                AccAltKeyNum = -224
            },

            new CaldarRecord
            {
                AccWkendN = 20202,
                AccApWkend = 20209,
                AccWeekN = 5,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 5,
                AccCln60Period = 1,
                AccCln61Week = 5,
                AccCln61Period = 1,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 1,
                AccWkend2N = 20020202,
                AccAltKeyNum = -223
            },

            new CaldarRecord
            {
                AccWkendN = 20209,
                AccApWkend = 20216,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20020209,
                AccAltKeyNum = -222
            },

            new CaldarRecord
            {
                AccWkendN = 20302,
                AccApWkend = 20309,
                AccWeekN = 9,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 9,
                AccCln60Period = 2,
                AccCln61Week = 9,
                AccCln61Period = 2,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 2,
                AccWkend2N = 20020302,
                AccAltKeyNum = -219
            },

            new CaldarRecord
            {
                AccWkendN = 20316,
                AccApWkend = 20323,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20020316,
                AccAltKeyNum = -217
            },

            new CaldarRecord
            {
                AccWkendN = 20413,
                AccApWkend = 20420,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20020413,
                AccAltKeyNum = -213
            },

            new CaldarRecord
            {
                AccWkendN = 20504,
                AccApWkend = 20511,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20020504,
                AccAltKeyNum = -210
            },

            new CaldarRecord
            {
                AccWkendN = 20518,
                AccApWkend = 20525,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20020518,
                AccAltKeyNum = -208
            },

            new CaldarRecord
            {
                AccWkendN = 20525,
                AccApWkend = 20601,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20020525,
                AccAltKeyNum = -207
            },

            new CaldarRecord
            {
                AccWkendN = 20608,
                AccApWkend = 20615,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20020608,
                AccAltKeyNum = -205
            },

            new CaldarRecord
            {
                AccWkendN = 20622,
                AccApWkend = 20629,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20020622,
                AccAltKeyNum = -203
            },

            new CaldarRecord
            {
                AccWkendN = 20629,
                AccApWkend = 20706,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20020629,
                AccAltKeyNum = -202
            },

            new CaldarRecord
            {
                AccWkendN = 20713,
                AccApWkend = 20720,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20020713,
                AccAltKeyNum = -200
            },

            new CaldarRecord
            {
                AccWkendN = 20720,
                AccApWkend = 20727,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20020720,
                AccAltKeyNum = -199
            },

            new CaldarRecord
            {
                AccWkendN = 20727,
                AccApWkend = 20803,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20020727,
                AccAltKeyNum = -198
            },

            new CaldarRecord
            {
                AccWkendN = 20803,
                AccApWkend = 20810,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20020803,
                AccAltKeyNum = -197
            },

            new CaldarRecord
            {
                AccWkendN = 20810,
                AccApWkend = 20817,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20020810,
                AccAltKeyNum = -196
            },

            new CaldarRecord
            {
                AccWkendN = 20817,
                AccApWkend = 20824,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20020817,
                AccAltKeyNum = -195
            },

            new CaldarRecord
            {
                AccWkendN = 20907,
                AccApWkend = 20914,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20020907,
                AccAltKeyNum = -192
            },

            new CaldarRecord
            {
                AccWkendN = 20914,
                AccApWkend = 20921,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20020914,
                AccAltKeyNum = -191
            },

            new CaldarRecord
            {
                AccWkendN = 20921,
                AccApWkend = 20928,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20020921,
                AccAltKeyNum = -190
            },

            new CaldarRecord
            {
                AccWkendN = 21005,
                AccApWkend = 21012,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20021005,
                AccAltKeyNum = -188
            },

            new CaldarRecord
            {
                AccWkendN = 21012,
                AccApWkend = 21019,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20021012,
                AccAltKeyNum = -187
            },

            new CaldarRecord
            {
                AccWkendN = 21026,
                AccApWkend = 21102,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20021026,
                AccAltKeyNum = -185
            },

            new CaldarRecord
            {
                AccWkendN = 21109,
                AccApWkend = 21116,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20021109,
                AccAltKeyNum = -183
            },

            new CaldarRecord
            {
                AccWkendN = 21116,
                AccApWkend = 21123,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20021116,
                AccAltKeyNum = -182
            },

            new CaldarRecord
            {
                AccWkendN = 21130,
                AccApWkend = 21207,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20021130,
                AccAltKeyNum = -180
            },

            new CaldarRecord
            {
                AccWkendN = 21214,
                AccApWkend = 21221,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20021214,
                AccAltKeyNum = -178
            },

            new CaldarRecord
            {
                AccWkendN = 21221,
                AccApWkend = 21228,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20021221,
                AccAltKeyNum = -177
            },

            new CaldarRecord
            {
                AccWkendN = 21228,
                AccApWkend = 30104,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20021228,
                AccAltKeyNum = -176
            },

            new CaldarRecord
            {
                AccWkendN = 30111,
                AccApWkend = 30118,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20030111,
                AccAltKeyNum = -166
            },

            new CaldarRecord
            {
                AccWkendN = 30125,
                AccApWkend = 30201,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20030125,
                AccAltKeyNum = -164
            },

            new CaldarRecord
            {
                AccWkendN = 30201,
                AccApWkend = 30208,
                AccWeekN = 5,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 5,
                AccCln60Period = 1,
                AccCln61Week = 5,
                AccCln61Period = 1,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 1,
                AccWkend2N = 20030201,
                AccAltKeyNum = -163
            },

            new CaldarRecord
            {
                AccWkendN = 30301,
                AccApWkend = 30308,
                AccWeekN = 9,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 9,
                AccCln60Period = 2,
                AccCln61Week = 9,
                AccCln61Period = 2,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 2,
                AccWkend2N = 20030301,
                AccAltKeyNum = -159
            },

            new CaldarRecord
            {
                AccWkendN = 30308,
                AccApWkend = 30315,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20030308,
                AccAltKeyNum = -158
            },

            new CaldarRecord
            {
                AccWkendN = 30322,
                AccApWkend = 30329,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20030322,
                AccAltKeyNum = -156
            },

            new CaldarRecord
            {
                AccWkendN = 30329,
                AccApWkend = 30405,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20030329,
                AccAltKeyNum = -155
            },

            new CaldarRecord
            {
                AccWkendN = 30405,
                AccApWkend = 30412,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20030405,
                AccAltKeyNum = -154
            },

            new CaldarRecord
            {
                AccWkendN = 30412,
                AccApWkend = 30419,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20030412,
                AccAltKeyNum = -153
            },

            new CaldarRecord
            {
                AccWkendN = 30419,
                AccApWkend = 30426,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20030419,
                AccAltKeyNum = -152
            },

            new CaldarRecord
            {
                AccWkendN = 30503,
                AccApWkend = 30510,
                AccWeekN = 18,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 18,
                AccCln60Period = 4,
                AccCln61Week = 18,
                AccCln61Period = 4,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 4,
                AccWkend2N = 20030503,
                AccAltKeyNum = -150
            },

            new CaldarRecord
            {
                AccWkendN = 30510,
                AccApWkend = 30517,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20030510,
                AccAltKeyNum = -149
            },

            new CaldarRecord
            {
                AccWkendN = 30517,
                AccApWkend = 30524,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20030517,
                AccAltKeyNum = -148
            },

            new CaldarRecord
            {
                AccWkendN = 30524,
                AccApWkend = 30531,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20030524,
                AccAltKeyNum = -147
            },

            new CaldarRecord
            {
                AccWkendN = 30607,
                AccApWkend = 30614,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20030607,
                AccAltKeyNum = -145
            },

            new CaldarRecord
            {
                AccWkendN = 30614,
                AccApWkend = 30621,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20030614,
                AccAltKeyNum = -144
            },

            new CaldarRecord
            {
                AccWkendN = 30621,
                AccApWkend = 30628,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20030621,
                AccAltKeyNum = -143
            },

            new CaldarRecord
            {
                AccWkendN = 30712,
                AccApWkend = 30719,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20030712,
                AccAltKeyNum = -140
            },

            new CaldarRecord
            {
                AccWkendN = 30719,
                AccApWkend = 30726,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20030719,
                AccAltKeyNum = -139
            },

            new CaldarRecord
            {
                AccWkendN = 30802,
                AccApWkend = 30809,
                AccWeekN = 31,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 31,
                AccCln60Period = 7,
                AccCln61Week = 31,
                AccCln61Period = 7,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 7,
                AccWkend2N = 20030802,
                AccAltKeyNum = -137
            },

            new CaldarRecord
            {
                AccWkendN = 30816,
                AccApWkend = 30823,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20030816,
                AccAltKeyNum = -135
            },

            new CaldarRecord
            {
                AccWkendN = 30906,
                AccApWkend = 30913,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20030906,
                AccAltKeyNum = -132
            },

            new CaldarRecord
            {
                AccWkendN = 30913,
                AccApWkend = 30920,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20030913,
                AccAltKeyNum = -131
            },

            new CaldarRecord
            {
                AccWkendN = 30920,
                AccApWkend = 30927,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20030920,
                AccAltKeyNum = -130
            },

            new CaldarRecord
            {
                AccWkendN = 30927,
                AccApWkend = 31004,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20030927,
                AccAltKeyNum = -129
            },

            new CaldarRecord
            {
                AccWkendN = 31004,
                AccApWkend = 31011,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20031004,
                AccAltKeyNum = -128
            },

            new CaldarRecord
            {
                AccWkendN = 31011,
                AccApWkend = 31018,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20031011,
                AccAltKeyNum = -127
            },

            new CaldarRecord
            {
                AccWkendN = 31025,
                AccApWkend = 31101,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20031025,
                AccAltKeyNum = -125
            },

            new CaldarRecord
            {
                AccWkendN = 31101,
                AccApWkend = 31108,
                AccWeekN = 44,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 44,
                AccCln60Period = 10,
                AccCln61Week = 44,
                AccCln61Period = 10,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 10,
                AccWkend2N = 20031101,
                AccAltKeyNum = -124
            },

            new CaldarRecord
            {
                AccWkendN = 31115,
                AccApWkend = 31122,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20031115,
                AccAltKeyNum = -122
            },

            new CaldarRecord
            {
                AccWkendN = 31122,
                AccApWkend = 31129,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20031122,
                AccAltKeyNum = -121
            },

            new CaldarRecord
            {
                AccWkendN = 31129,
                AccApWkend = 31206,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20031129,
                AccAltKeyNum = -120
            },

            new CaldarRecord
            {
                AccWkendN = 31206,
                AccApWkend = 31213,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20031206,
                AccAltKeyNum = -119
            },

            new CaldarRecord
            {
                AccWkendN = 31227,
                AccApWkend = 40103,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20031227,
                AccAltKeyNum = -116
            },

            new CaldarRecord
            {
                AccWkendN = 40103,
                AccApWkend = 40110,
                AccWeekN = 53,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 53,
                AccCln60Period = 12,
                AccCln61Week = 53,
                AccCln61Period = 12,
                AccCln7XWeek = 53,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 53,
                AccCln6XPeriod = 12,
                AccWkend2N = 20040103,
                AccAltKeyNum = -115
            },

            new CaldarRecord
            {
                AccWkendN = 40110,
                AccApWkend = 40117,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20040110,
                AccAltKeyNum = -107
            },

            new CaldarRecord
            {
                AccWkendN = 40124,
                AccApWkend = 40131,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20040124,
                AccAltKeyNum = -105
            },

            new CaldarRecord
            {
                AccWkendN = 40221,
                AccApWkend = 40228,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20040221,
                AccAltKeyNum = -101
            },

            new CaldarRecord
            {
                AccWkendN = 40228,
                AccApWkend = 40306,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20040228,
                AccAltKeyNum = -100
            },

            new CaldarRecord
            {
                AccWkendN = 40327,
                AccApWkend = 40403,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20040327,
                AccAltKeyNum = -96
            },

            new CaldarRecord
            {
                AccWkendN = 40403,
                AccApWkend = 40410,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20040403,
                AccAltKeyNum = -95
            },

            new CaldarRecord
            {
                AccWkendN = 40410,
                AccApWkend = 40417,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20040410,
                AccAltKeyNum = -94
            },

            new CaldarRecord
            {
                AccWkendN = 40417,
                AccApWkend = 40424,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20040417,
                AccAltKeyNum = -93
            },

            new CaldarRecord
            {
                AccWkendN = 40424,
                AccApWkend = 40501,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20040424,
                AccAltKeyNum = -92
            },

            new CaldarRecord
            {
                AccWkendN = 40501,
                AccApWkend = 40508,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20040501,
                AccAltKeyNum = -91
            },

            new CaldarRecord
            {
                AccWkendN = 40508,
                AccApWkend = 40515,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20040508,
                AccAltKeyNum = -90
            },

            new CaldarRecord
            {
                AccWkendN = 40522,
                AccApWkend = 40529,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20040522,
                AccAltKeyNum = -88
            },

            new CaldarRecord
            {
                AccWkendN = 40612,
                AccApWkend = 40619,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20040612,
                AccAltKeyNum = -85
            },

            new CaldarRecord
            {
                AccWkendN = 40619,
                AccApWkend = 40626,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20040619,
                AccAltKeyNum = -84
            },

            new CaldarRecord
            {
                AccWkendN = 40626,
                AccApWkend = 40703,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20040626,
                AccAltKeyNum = -83
            },

            new CaldarRecord
            {
                AccWkendN = 40710,
                AccApWkend = 40717,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20040710,
                AccAltKeyNum = -81
            },

            new CaldarRecord
            {
                AccWkendN = 40731,
                AccApWkend = 40807,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20040731,
                AccAltKeyNum = -78
            },

            new CaldarRecord
            {
                AccWkendN = 40828,
                AccApWkend = 40904,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20040828,
                AccAltKeyNum = -74
            },

            new CaldarRecord
            {
                AccWkendN = 40904,
                AccApWkend = 40911,
                AccWeekN = 35,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 35,
                AccCln60Period = 9,
                AccCln61Week = 35,
                AccCln61Period = 9,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 9,
                AccWkend2N = 20040904,
                AccAltKeyNum = -73
            },

            new CaldarRecord
            {
                AccWkendN = 40911,
                AccApWkend = 40918,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20040911,
                AccAltKeyNum = -72
            },

            new CaldarRecord
            {
                AccWkendN = 40918,
                AccApWkend = 40925,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20040918,
                AccAltKeyNum = -71
            },

            new CaldarRecord
            {
                AccWkendN = 41002,
                AccApWkend = 41009,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20041002,
                AccAltKeyNum = -69
            },

            new CaldarRecord
            {
                AccWkendN = 41009,
                AccApWkend = 41016,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20041009,
                AccAltKeyNum = -68
            },

            new CaldarRecord
            {
                AccWkendN = 41016,
                AccApWkend = 41023,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20041016,
                AccAltKeyNum = -67
            },

            new CaldarRecord
            {
                AccWkendN = 220108,
                AccApWkend = 220115,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20220108,
                AccAltKeyNum = 973
            },

            new CaldarRecord
            {
                AccWkendN = 220115,
                AccApWkend = 220122,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20220115,
                AccAltKeyNum = 974
            },

            new CaldarRecord
            {
                AccWkendN = 220122,
                AccApWkend = 220129,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20220122,
                AccAltKeyNum = 975
            },

            new CaldarRecord
            {
                AccWkendN = 220129,
                AccApWkend = 220205,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20220129,
                AccAltKeyNum = 976
            },

            new CaldarRecord
            {
                AccWkendN = 220205,
                AccApWkend = 220212,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20220205,
                AccAltKeyNum = 977
            },

            new CaldarRecord
            {
                AccWkendN = 220212,
                AccApWkend = 220219,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20220212,
                AccAltKeyNum = 978
            },

            new CaldarRecord
            {
                AccWkendN = 220402,
                AccApWkend = 220409,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20220402,
                AccAltKeyNum = 985
            },

            new CaldarRecord
            {
                AccWkendN = 220409,
                AccApWkend = 220416,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20220409,
                AccAltKeyNum = 986
            },

            new CaldarRecord
            {
                AccWkendN = 220416,
                AccApWkend = 220423,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20220416,
                AccAltKeyNum = 987
            },

            new CaldarRecord
            {
                AccWkendN = 220514,
                AccApWkend = 220521,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20220514,
                AccAltKeyNum = 991
            },

            new CaldarRecord
            {
                AccWkendN = 220521,
                AccApWkend = 220528,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20220521,
                AccAltKeyNum = 992
            },

            new CaldarRecord
            {
                AccWkendN = 220528,
                AccApWkend = 220604,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20220528,
                AccAltKeyNum = 993
            },

            new CaldarRecord
            {
                AccWkendN = 220604,
                AccApWkend = 220611,
                AccWeekN = 22,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 22,
                AccCln60Period = 6,
                AccCln61Week = 22,
                AccCln61Period = 6,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 6,
                AccWkend2N = 20220604,
                AccAltKeyNum = 994
            },

            new CaldarRecord
            {
                AccWkendN = 220611,
                AccApWkend = 220618,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20220611,
                AccAltKeyNum = 995
            },

            new CaldarRecord
            {
                AccWkendN = 220618,
                AccApWkend = 220625,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20220618,
                AccAltKeyNum = 996
            },

            new CaldarRecord
            {
                AccWkendN = 220625,
                AccApWkend = 220702,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20220625,
                AccAltKeyNum = 997
            },

            new CaldarRecord
            {
                AccWkendN = 220702,
                AccApWkend = 220709,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20220702,
                AccAltKeyNum = 998
            },

            new CaldarRecord
            {
                AccWkendN = 220709,
                AccApWkend = 220716,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20220709,
                AccAltKeyNum = 999
            },

            new CaldarRecord
            {
                AccWkendN = 220716,
                AccApWkend = 220723,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20220716,
                AccAltKeyNum = 1000
            },

            new CaldarRecord
            {
                AccWkendN = 220723,
                AccApWkend = 220730,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20220723,
                AccAltKeyNum = 1001
            },

            new CaldarRecord
            {
                AccWkendN = 220730,
                AccApWkend = 220806,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20220730,
                AccAltKeyNum = 1002
            },

            new CaldarRecord
            {
                AccWkendN = 220806,
                AccApWkend = 220813,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20220806,
                AccAltKeyNum = 1003
            },

            new CaldarRecord
            {
                AccWkendN = 220813,
                AccApWkend = 220820,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20220813,
                AccAltKeyNum = 1004
            },

            new CaldarRecord
            {
                AccWkendN = 220820,
                AccApWkend = 220827,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20220820,
                AccAltKeyNum = 1005
            },

            new CaldarRecord
            {
                AccWkendN = 220827,
                AccApWkend = 220903,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20220827,
                AccAltKeyNum = 1006
            },

            new CaldarRecord
            {
                AccWkendN = 220903,
                AccApWkend = 220910,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20220903,
                AccAltKeyNum = 1007
            },

            new CaldarRecord
            {
                AccWkendN = 220910,
                AccApWkend = 220917,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20220910,
                AccAltKeyNum = 1008
            },

            new CaldarRecord
            {
                AccWkendN = 221008,
                AccApWkend = 221015,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20221008,
                AccAltKeyNum = 1012
            },

            new CaldarRecord
            {
                AccWkendN = 221015,
                AccApWkend = 221022,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20221015,
                AccAltKeyNum = 1013
            },

            new CaldarRecord
            {
                AccWkendN = 221022,
                AccApWkend = 221029,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20221022,
                AccAltKeyNum = 1014
            },

            new CaldarRecord
            {
                AccWkendN = 221029,
                AccApWkend = 221105,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20221029,
                AccAltKeyNum = 1015
            },

            new CaldarRecord
            {
                AccWkendN = 221105,
                AccApWkend = 221112,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 20221105,
                AccAltKeyNum = 1016
            },

            new CaldarRecord
            {
                AccWkendN = 221112,
                AccApWkend = 221119,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20221112,
                AccAltKeyNum = 1017
            },

            new CaldarRecord
            {
                AccWkendN = 221119,
                AccApWkend = 221126,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20221119,
                AccAltKeyNum = 1018
            },

            new CaldarRecord
            {
                AccWkendN = 221126,
                AccApWkend = 221203,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20221126,
                AccAltKeyNum = 1019
            },

            new CaldarRecord
            {
                AccWkendN = 221203,
                AccApWkend = 221210,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20221203,
                AccAltKeyNum = 1020
            },

            new CaldarRecord
            {
                AccWkendN = 221210,
                AccApWkend = 221217,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20221210,
                AccAltKeyNum = 1021
            },

            new CaldarRecord
            {
                AccWkendN = 221217,
                AccApWkend = 221224,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20221217,
                AccAltKeyNum = 1022
            },

            new CaldarRecord
            {
                AccWkendN = 221224,
                AccApWkend = 221231,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20221224,
                AccAltKeyNum = 1023
            },

            new CaldarRecord
            {
                AccWkendN = 210313,
                AccApWkend = 210320,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20210313,
                AccAltKeyNum = 922
            },

            new CaldarRecord
            {
                AccWkendN = 210320,
                AccApWkend = 210327,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20210320,
                AccAltKeyNum = 923
            },

            new CaldarRecord
            {
                AccWkendN = 210327,
                AccApWkend = 210403,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20210327,
                AccAltKeyNum = 924
            },

            new CaldarRecord
            {
                AccWkendN = 210403,
                AccApWkend = 210410,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20210403,
                AccAltKeyNum = 925
            },

            new CaldarRecord
            {
                AccWkendN = 210410,
                AccApWkend = 210417,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20210410,
                AccAltKeyNum = 926
            },

            new CaldarRecord
            {
                AccWkendN = 210417,
                AccApWkend = 210424,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20210417,
                AccAltKeyNum = 927
            },

            new CaldarRecord
            {
                AccWkendN = 210515,
                AccApWkend = 210522,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20210515,
                AccAltKeyNum = 931
            },

            new CaldarRecord
            {
                AccWkendN = 210522,
                AccApWkend = 210529,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20210522,
                AccAltKeyNum = 932
            },

            new CaldarRecord
            {
                AccWkendN = 210529,
                AccApWkend = 210605,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20210529,
                AccAltKeyNum = 933
            },

            new CaldarRecord
            {
                AccWkendN = 210717,
                AccApWkend = 210724,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20210717,
                AccAltKeyNum = 940
            },

            new CaldarRecord
            {
                AccWkendN = 210724,
                AccApWkend = 210731,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20210724,
                AccAltKeyNum = 941
            },

            new CaldarRecord
            {
                AccWkendN = 210731,
                AccApWkend = 210807,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20210731,
                AccAltKeyNum = 942
            },

            new CaldarRecord
            {
                AccWkendN = 210918,
                AccApWkend = 210925,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20210918,
                AccAltKeyNum = 949
            },

            new CaldarRecord
            {
                AccWkendN = 210925,
                AccApWkend = 211002,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20210925,
                AccAltKeyNum = 950
            },

            new CaldarRecord
            {
                AccWkendN = 211002,
                AccApWkend = 211009,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20211002,
                AccAltKeyNum = 951
            },

            new CaldarRecord
            {
                AccWkendN = 211030,
                AccApWkend = 211106,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20211030,
                AccAltKeyNum = 955
            },

            new CaldarRecord
            {
                AccWkendN = 211106,
                AccApWkend = 211113,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 20211106,
                AccAltKeyNum = 956
            },

            new CaldarRecord
            {
                AccWkendN = 211113,
                AccApWkend = 211120,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20211113,
                AccAltKeyNum = 957
            },

            new CaldarRecord
            {
                AccWkendN = 211120,
                AccApWkend = 211127,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20211120,
                AccAltKeyNum = 958
            },

            new CaldarRecord
            {
                AccWkendN = 211127,
                AccApWkend = 211204,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20211127,
                AccAltKeyNum = 959
            },

            new CaldarRecord
            {
                AccWkendN = 211204,
                AccApWkend = 211211,
                AccWeekN = 48,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 48,
                AccCln60Period = 12,
                AccCln61Week = 48,
                AccCln61Period = 12,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 12,
                AccWkend2N = 20211204,
                AccAltKeyNum = 960
            },

            new CaldarRecord
            {
                AccWkendN = 220101,
                AccApWkend = 220108,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20220101,
                AccAltKeyNum = 964
            },

            new CaldarRecord
            {
                AccWkendN = 220219,
                AccApWkend = 220226,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20220219,
                AccAltKeyNum = 979
            },

            new CaldarRecord
            {
                AccWkendN = 220226,
                AccApWkend = 220305,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20220226,
                AccAltKeyNum = 980
            },

            new CaldarRecord
            {
                AccWkendN = 220305,
                AccApWkend = 220312,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 20220305,
                AccAltKeyNum = 981
            },

            new CaldarRecord
            {
                AccWkendN = 220312,
                AccApWkend = 220319,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20220312,
                AccAltKeyNum = 982
            },

            new CaldarRecord
            {
                AccWkendN = 220319,
                AccApWkend = 220326,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20220319,
                AccAltKeyNum = 983
            },

            new CaldarRecord
            {
                AccWkendN = 220326,
                AccApWkend = 220402,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20220326,
                AccAltKeyNum = 984
            },

            new CaldarRecord
            {
                AccWkendN = 220423,
                AccApWkend = 220430,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20220423,
                AccAltKeyNum = 988
            },

            new CaldarRecord
            {
                AccWkendN = 220430,
                AccApWkend = 220507,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20220430,
                AccAltKeyNum = 989
            },

            new CaldarRecord
            {
                AccWkendN = 220507,
                AccApWkend = 220514,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20220507,
                AccAltKeyNum = 990
            },

            new CaldarRecord
            {
                AccWkendN = 220917,
                AccApWkend = 220924,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20220917,
                AccAltKeyNum = 1009
            },

            new CaldarRecord
            {
                AccWkendN = 220924,
                AccApWkend = 221001,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20220924,
                AccAltKeyNum = 1010
            },

            new CaldarRecord
            {
                AccWkendN = 221001,
                AccApWkend = 221008,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20221001,
                AccAltKeyNum = 1011
            },

            new CaldarRecord
            {
                AccWkendN = 221231,
                AccApWkend = 230107,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20221231,
                AccAltKeyNum = 1024
            },

            new CaldarRecord
            {
                AccWkendN = 210130,
                AccApWkend = 210206,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20210130,
                AccAltKeyNum = 916
            },

            new CaldarRecord
            {
                AccWkendN = 210206,
                AccApWkend = 210213,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20210206,
                AccAltKeyNum = 917
            },

            new CaldarRecord
            {
                AccWkendN = 210213,
                AccApWkend = 210220,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20210213,
                AccAltKeyNum = 918
            },

            new CaldarRecord
            {
                AccWkendN = 210424,
                AccApWkend = 210501,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20210424,
                AccAltKeyNum = 928
            },

            new CaldarRecord
            {
                AccWkendN = 210501,
                AccApWkend = 210508,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20210501,
                AccAltKeyNum = 929
            },

            new CaldarRecord
            {
                AccWkendN = 210508,
                AccApWkend = 210515,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20210508,
                AccAltKeyNum = 930
            },

            new CaldarRecord
            {
                AccWkendN = 210605,
                AccApWkend = 210612,
                AccWeekN = 22,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 22,
                AccCln60Period = 6,
                AccCln61Week = 22,
                AccCln61Period = 6,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 6,
                AccWkend2N = 20210605,
                AccAltKeyNum = 934
            },

            new CaldarRecord
            {
                AccWkendN = 210612,
                AccApWkend = 210619,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20210612,
                AccAltKeyNum = 935
            },

            new CaldarRecord
            {
                AccWkendN = 210619,
                AccApWkend = 210626,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20210619,
                AccAltKeyNum = 936
            },

            new CaldarRecord
            {
                AccWkendN = 210626,
                AccApWkend = 210703,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20210626,
                AccAltKeyNum = 937
            },

            new CaldarRecord
            {
                AccWkendN = 210703,
                AccApWkend = 210710,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20210703,
                AccAltKeyNum = 938
            },

            new CaldarRecord
            {
                AccWkendN = 210710,
                AccApWkend = 210717,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20210710,
                AccAltKeyNum = 939
            },

            new CaldarRecord
            {
                AccWkendN = 210807,
                AccApWkend = 210814,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20210807,
                AccAltKeyNum = 943
            },

            new CaldarRecord
            {
                AccWkendN = 210814,
                AccApWkend = 210821,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20210814,
                AccAltKeyNum = 944
            },

            new CaldarRecord
            {
                AccWkendN = 210821,
                AccApWkend = 210828,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20210821,
                AccAltKeyNum = 945
            },

            new CaldarRecord
            {
                AccWkendN = 210828,
                AccApWkend = 210904,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20210828,
                AccAltKeyNum = 946
            },

            new CaldarRecord
            {
                AccWkendN = 210904,
                AccApWkend = 210911,
                AccWeekN = 35,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 35,
                AccCln60Period = 9,
                AccCln61Week = 35,
                AccCln61Period = 9,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 9,
                AccWkend2N = 20210904,
                AccAltKeyNum = 947
            },

            new CaldarRecord
            {
                AccWkendN = 210911,
                AccApWkend = 210918,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20210911,
                AccAltKeyNum = 948
            },

            new CaldarRecord
            {
                AccWkendN = 211009,
                AccApWkend = 211016,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20211009,
                AccAltKeyNum = 952
            },

            new CaldarRecord
            {
                AccWkendN = 211016,
                AccApWkend = 211023,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20211016,
                AccAltKeyNum = 953
            },

            new CaldarRecord
            {
                AccWkendN = 211023,
                AccApWkend = 211030,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20211023,
                AccAltKeyNum = 954
            },

            new CaldarRecord
            {
                AccWkendN = 211211,
                AccApWkend = 211218,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20211211,
                AccAltKeyNum = 961
            },

            new CaldarRecord
            {
                AccWkendN = 211218,
                AccApWkend = 211225,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20211218,
                AccAltKeyNum = 962
            },

            new CaldarRecord
            {
                AccWkendN = 211225,
                AccApWkend = 220101,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20211225,
                AccAltKeyNum = 963
            },

            new CaldarRecord
            {
                AccWkendN = 210109,
                AccApWkend = 210116,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20210109,
                AccAltKeyNum = 913
            },

            new CaldarRecord
            {
                AccWkendN = 210116,
                AccApWkend = 210123,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20210116,
                AccAltKeyNum = 914
            },

            new CaldarRecord
            {
                AccWkendN = 210123,
                AccApWkend = 210130,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20210123,
                AccAltKeyNum = 915
            },

            new CaldarRecord
            {
                AccWkendN = 210220,
                AccApWkend = 210227,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20210220,
                AccAltKeyNum = 919
            },

            new CaldarRecord
            {
                AccWkendN = 210227,
                AccApWkend = 210306,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20210227,
                AccAltKeyNum = 920
            },

            new CaldarRecord
            {
                AccWkendN = 210306,
                AccApWkend = 210313,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 20210306,
                AccAltKeyNum = 921
            },

            new CaldarRecord
            {
                AccWkendN = 230422,
                AccApWkend = 230429,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20230422,
                AccAltKeyNum = 1048
            },

            new CaldarRecord
            {
                AccWkendN = 230429,
                AccApWkend = 230506,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20230429,
                AccAltKeyNum = 1049
            },

            new CaldarRecord
            {
                AccWkendN = 230506,
                AccApWkend = 230513,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20230506,
                AccAltKeyNum = 1050
            },

            new CaldarRecord
            {
                AccWkendN = 230916,
                AccApWkend = 230923,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20230916,
                AccAltKeyNum = 1069
            },

            new CaldarRecord
            {
                AccWkendN = 230923,
                AccApWkend = 230930,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20230923,
                AccAltKeyNum = 1070
            },

            new CaldarRecord
            {
                AccWkendN = 230930,
                AccApWkend = 231007,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20230930,
                AccAltKeyNum = 1071
            },

            new CaldarRecord
            {
                AccWkendN = 231028,
                AccApWkend = 231104,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20231028,
                AccAltKeyNum = 1075
            },

            new CaldarRecord
            {
                AccWkendN = 231104,
                AccApWkend = 231111,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 20231104,
                AccAltKeyNum = 1076
            },

            new CaldarRecord
            {
                AccWkendN = 231111,
                AccApWkend = 231118,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20231111,
                AccAltKeyNum = 1077
            },

            new CaldarRecord
            {
                AccWkendN = 231118,
                AccApWkend = 231125,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20231118,
                AccAltKeyNum = 1078
            },

            new CaldarRecord
            {
                AccWkendN = 231125,
                AccApWkend = 231202,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20231125,
                AccAltKeyNum = 1079
            },

            new CaldarRecord
            {
                AccWkendN = 231202,
                AccApWkend = 231209,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20231202,
                AccAltKeyNum = 1080
            },

            new CaldarRecord
            {
                AccWkendN = 230107,
                AccApWkend = 230114,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20230107,
                AccAltKeyNum = 1033
            },

            new CaldarRecord
            {
                AccWkendN = 230114,
                AccApWkend = 230121,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20230114,
                AccAltKeyNum = 1034
            },

            new CaldarRecord
            {
                AccWkendN = 230121,
                AccApWkend = 230128,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20230121,
                AccAltKeyNum = 1035
            },

            new CaldarRecord
            {
                AccWkendN = 115,
                AccApWkend = 122,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20000115,
                AccAltKeyNum = -346
            },

            new CaldarRecord
            {
                AccWkendN = 122,
                AccApWkend = 129,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20000122,
                AccAltKeyNum = -345
            },

            new CaldarRecord
            {
                AccWkendN = 129,
                AccApWkend = 205,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20000129,
                AccAltKeyNum = -344
            },

            new CaldarRecord
            {
                AccWkendN = 212,
                AccApWkend = 219,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20000212,
                AccAltKeyNum = -342
            },

            new CaldarRecord
            {
                AccWkendN = 226,
                AccApWkend = 304,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20000226,
                AccAltKeyNum = -340
            },

            new CaldarRecord
            {
                AccWkendN = 311,
                AccApWkend = 318,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20000311,
                AccAltKeyNum = -338
            },

            new CaldarRecord
            {
                AccWkendN = 318,
                AccApWkend = 325,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20000318,
                AccAltKeyNum = -337
            },

            new CaldarRecord
            {
                AccWkendN = 325,
                AccApWkend = 401,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20000325,
                AccAltKeyNum = -336
            },

            new CaldarRecord
            {
                AccWkendN = 401,
                AccApWkend = 408,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20000401,
                AccAltKeyNum = -335
            },

            new CaldarRecord
            {
                AccWkendN = 408,
                AccApWkend = 415,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20000408,
                AccAltKeyNum = -334
            },

            new CaldarRecord
            {
                AccWkendN = 415,
                AccApWkend = 422,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20000415,
                AccAltKeyNum = -333
            },

            new CaldarRecord
            {
                AccWkendN = 422,
                AccApWkend = 429,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20000422,
                AccAltKeyNum = -332
            },

            new CaldarRecord
            {
                AccWkendN = 506,
                AccApWkend = 513,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20000506,
                AccAltKeyNum = -330
            },

            new CaldarRecord
            {
                AccWkendN = 527,
                AccApWkend = 603,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20000527,
                AccAltKeyNum = -327
            },

            new CaldarRecord
            {
                AccWkendN = 610,
                AccApWkend = 617,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20000610,
                AccAltKeyNum = -325
            },

            new CaldarRecord
            {
                AccWkendN = 624,
                AccApWkend = 701,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20000624,
                AccAltKeyNum = -323
            },

            new CaldarRecord
            {
                AccWkendN = 708,
                AccApWkend = 715,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20000708,
                AccAltKeyNum = -321
            },

            new CaldarRecord
            {
                AccWkendN = 715,
                AccApWkend = 722,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20000715,
                AccAltKeyNum = -320
            },

            new CaldarRecord
            {
                AccWkendN = 722,
                AccApWkend = 729,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20000722,
                AccAltKeyNum = -319
            },

            new CaldarRecord
            {
                AccWkendN = 805,
                AccApWkend = 812,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20000805,
                AccAltKeyNum = -317
            },

            new CaldarRecord
            {
                AccWkendN = 812,
                AccApWkend = 819,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20000812,
                AccAltKeyNum = -316
            },

            new CaldarRecord
            {
                AccWkendN = 819,
                AccApWkend = 826,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20000819,
                AccAltKeyNum = -315
            },

            new CaldarRecord
            {
                AccWkendN = 826,
                AccApWkend = 902,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20000826,
                AccAltKeyNum = -314
            },

            new CaldarRecord
            {
                AccWkendN = 902,
                AccApWkend = 909,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20000902,
                AccAltKeyNum = -313
            },

            new CaldarRecord
            {
                AccWkendN = 916,
                AccApWkend = 923,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20000916,
                AccAltKeyNum = -311
            },

            new CaldarRecord
            {
                AccWkendN = 923,
                AccApWkend = 930,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20000923,
                AccAltKeyNum = -310
            },

            new CaldarRecord
            {
                AccWkendN = 930,
                AccApWkend = 1007,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20000930,
                AccAltKeyNum = -309
            },

            new CaldarRecord
            {
                AccWkendN = 1007,
                AccApWkend = 1014,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20001007,
                AccAltKeyNum = -308
            },

            new CaldarRecord
            {
                AccWkendN = 1021,
                AccApWkend = 1028,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20001021,
                AccAltKeyNum = -306
            },

            new CaldarRecord
            {
                AccWkendN = 1104,
                AccApWkend = 1111,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 20001104,
                AccAltKeyNum = -304
            },

            new CaldarRecord
            {
                AccWkendN = 1111,
                AccApWkend = 1118,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20001111,
                AccAltKeyNum = -303
            },

            new CaldarRecord
            {
                AccWkendN = 1118,
                AccApWkend = 1125,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20001118,
                AccAltKeyNum = -302
            },

            new CaldarRecord
            {
                AccWkendN = 1125,
                AccApWkend = 1202,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20001125,
                AccAltKeyNum = -301
            },

            new CaldarRecord
            {
                AccWkendN = 1209,
                AccApWkend = 1216,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20001209,
                AccAltKeyNum = -299
            },

            new CaldarRecord
            {
                AccWkendN = 1223,
                AccApWkend = 1230,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20001223,
                AccAltKeyNum = -297
            },

            new CaldarRecord
            {
                AccWkendN = 1230,
                AccApWkend = 10106,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20001230,
                AccAltKeyNum = -296
            },

            new CaldarRecord
            {
                AccWkendN = 729,
                AccApWkend = 805,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20000729,
                AccAltKeyNum = -318
            },

            new CaldarRecord
            {
                AccWkendN = 909,
                AccApWkend = 916,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20000909,
                AccAltKeyNum = -312
            },

            new CaldarRecord
            {
                AccWkendN = 1014,
                AccApWkend = 1021,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20001014,
                AccAltKeyNum = -307
            },

            new CaldarRecord
            {
                AccWkendN = 1028,
                AccApWkend = 1104,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20001028,
                AccAltKeyNum = -305
            },

            new CaldarRecord
            {
                AccWkendN = 1202,
                AccApWkend = 1209,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20001202,
                AccAltKeyNum = -300
            },

            new CaldarRecord
            {
                AccWkendN = 1216,
                AccApWkend = 1223,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20001216,
                AccAltKeyNum = -298
            },

            new CaldarRecord
            {
                AccWkendN = 108,
                AccApWkend = 115,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20000108,
                AccAltKeyNum = -347
            },

            new CaldarRecord
            {
                AccWkendN = 205,
                AccApWkend = 212,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20000205,
                AccAltKeyNum = -343
            },

            new CaldarRecord
            {
                AccWkendN = 219,
                AccApWkend = 226,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20000219,
                AccAltKeyNum = -341
            },

            new CaldarRecord
            {
                AccWkendN = 304,
                AccApWkend = 311,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 20000304,
                AccAltKeyNum = -339
            },

            new CaldarRecord
            {
                AccWkendN = 429,
                AccApWkend = 506,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20000429,
                AccAltKeyNum = -331
            },

            new CaldarRecord
            {
                AccWkendN = 513,
                AccApWkend = 520,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20000513,
                AccAltKeyNum = -329
            },

            new CaldarRecord
            {
                AccWkendN = 520,
                AccApWkend = 527,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20000520,
                AccAltKeyNum = -328
            },

            new CaldarRecord
            {
                AccWkendN = 603,
                AccApWkend = 610,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20000603,
                AccAltKeyNum = -326
            },

            new CaldarRecord
            {
                AccWkendN = 617,
                AccApWkend = 624,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20000617,
                AccAltKeyNum = -324
            },

            new CaldarRecord
            {
                AccWkendN = 701,
                AccApWkend = 708,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20000701,
                AccAltKeyNum = -322
            },

            new CaldarRecord
            {
                AccWkendN = 230128,
                AccApWkend = 230204,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20230128,
                AccAltKeyNum = 1036
            },

            new CaldarRecord
            {
                AccWkendN = 230204,
                AccApWkend = 230211,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 20230204,
                AccAltKeyNum = 1037
            },

            new CaldarRecord
            {
                AccWkendN = 230211,
                AccApWkend = 230218,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20230211,
                AccAltKeyNum = 1038
            },

            new CaldarRecord
            {
                AccWkendN = 230218,
                AccApWkend = 230225,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20230218,
                AccAltKeyNum = 1039
            },

            new CaldarRecord
            {
                AccWkendN = 230225,
                AccApWkend = 230304,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20230225,
                AccAltKeyNum = 1040
            },

            new CaldarRecord
            {
                AccWkendN = 230304,
                AccApWkend = 230311,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 20230304,
                AccAltKeyNum = 1041
            },

            new CaldarRecord
            {
                AccWkendN = 230311,
                AccApWkend = 230318,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20230311,
                AccAltKeyNum = 1042
            },

            new CaldarRecord
            {
                AccWkendN = 230318,
                AccApWkend = 230325,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20230318,
                AccAltKeyNum = 1043
            },

            new CaldarRecord
            {
                AccWkendN = 230325,
                AccApWkend = 230401,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20230325,
                AccAltKeyNum = 1044
            },

            new CaldarRecord
            {
                AccWkendN = 230401,
                AccApWkend = 230408,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20230401,
                AccAltKeyNum = 1045
            },

            new CaldarRecord
            {
                AccWkendN = 230408,
                AccApWkend = 230415,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20230408,
                AccAltKeyNum = 1046
            },

            new CaldarRecord
            {
                AccWkendN = 230415,
                AccApWkend = 230422,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20230415,
                AccAltKeyNum = 1047
            },

            new CaldarRecord
            {
                AccWkendN = 230513,
                AccApWkend = 230520,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20230513,
                AccAltKeyNum = 1051
            },

            new CaldarRecord
            {
                AccWkendN = 230520,
                AccApWkend = 230527,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20230520,
                AccAltKeyNum = 1052
            },

            new CaldarRecord
            {
                AccWkendN = 230527,
                AccApWkend = 230603,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20230527,
                AccAltKeyNum = 1053
            },

            new CaldarRecord
            {
                AccWkendN = 230603,
                AccApWkend = 230610,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20230603,
                AccAltKeyNum = 1054
            },

            new CaldarRecord
            {
                AccWkendN = 230610,
                AccApWkend = 230617,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20230610,
                AccAltKeyNum = 1055
            },

            new CaldarRecord
            {
                AccWkendN = 230617,
                AccApWkend = 230624,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20230617,
                AccAltKeyNum = 1056
            },

            new CaldarRecord
            {
                AccWkendN = 230624,
                AccApWkend = 230701,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20230624,
                AccAltKeyNum = 1057
            },

            new CaldarRecord
            {
                AccWkendN = 230701,
                AccApWkend = 230708,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20230701,
                AccAltKeyNum = 1058
            },

            new CaldarRecord
            {
                AccWkendN = 230708,
                AccApWkend = 230715,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20230708,
                AccAltKeyNum = 1059
            },

            new CaldarRecord
            {
                AccWkendN = 230715,
                AccApWkend = 230722,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 20230715,
                AccAltKeyNum = 1060
            },

            new CaldarRecord
            {
                AccWkendN = 230722,
                AccApWkend = 230729,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 20230722,
                AccAltKeyNum = 1061
            },

            new CaldarRecord
            {
                AccWkendN = 230729,
                AccApWkend = 230805,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20230729,
                AccAltKeyNum = 1062
            },

            new CaldarRecord
            {
                AccWkendN = 230805,
                AccApWkend = 230812,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 20230805,
                AccAltKeyNum = 1063
            },

            new CaldarRecord
            {
                AccWkendN = 230812,
                AccApWkend = 230819,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20230812,
                AccAltKeyNum = 1064
            },

            new CaldarRecord
            {
                AccWkendN = 230819,
                AccApWkend = 230826,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20230819,
                AccAltKeyNum = 1065
            },

            new CaldarRecord
            {
                AccWkendN = 230826,
                AccApWkend = 230902,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20230826,
                AccAltKeyNum = 1066
            },

            new CaldarRecord
            {
                AccWkendN = 230902,
                AccApWkend = 230909,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20230902,
                AccAltKeyNum = 1067
            },

            new CaldarRecord
            {
                AccWkendN = 230909,
                AccApWkend = 230916,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20230909,
                AccAltKeyNum = 1068
            },

            new CaldarRecord
            {
                AccWkendN = 231007,
                AccApWkend = 231014,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20231007,
                AccAltKeyNum = 1072
            },

            new CaldarRecord
            {
                AccWkendN = 231014,
                AccApWkend = 231021,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20231014,
                AccAltKeyNum = 1073
            },

            new CaldarRecord
            {
                AccWkendN = 231021,
                AccApWkend = 231028,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20231021,
                AccAltKeyNum = 1074
            },

            new CaldarRecord
            {
                AccWkendN = 231209,
                AccApWkend = 231216,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20231209,
                AccAltKeyNum = 1081
            },

            new CaldarRecord
            {
                AccWkendN = 231216,
                AccApWkend = 231223,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20231216,
                AccAltKeyNum = 1082
            },

            new CaldarRecord
            {
                AccWkendN = 231223,
                AccApWkend = 231230,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20231223,
                AccAltKeyNum = 1083
            },

            new CaldarRecord
            {
                AccWkendN = 231230,
                AccApWkend = 240106,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20231230,
                AccAltKeyNum = 1084
            },

            new CaldarRecord
            {
                AccWkendN = 240217,
                AccApWkend = 240224,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 20240217,
                AccAltKeyNum = 1099
            },

            new CaldarRecord
            {
                AccWkendN = 240224,
                AccApWkend = 240302,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 20240224,
                AccAltKeyNum = 1100
            },

            new CaldarRecord
            {
                AccWkendN = 240302,
                AccApWkend = 240309,
                AccWeekN = 9,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 9,
                AccCln60Period = 2,
                AccCln61Week = 9,
                AccCln61Period = 2,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 2,
                AccWkend2N = 20240302,
                AccAltKeyNum = 1101
            },

            new CaldarRecord
            {
                AccWkendN = 240309,
                AccApWkend = 240316,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 20240309,
                AccAltKeyNum = 1102
            },

            new CaldarRecord
            {
                AccWkendN = 240316,
                AccApWkend = 240323,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 20240316,
                AccAltKeyNum = 1103
            },

            new CaldarRecord
            {
                AccWkendN = 240323,
                AccApWkend = 240330,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 20240323,
                AccAltKeyNum = 1104
            },

            new CaldarRecord
            {
                AccWkendN = 240330,
                AccApWkend = 240406,
                AccWeekN = 13,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 13,
                AccCln60Period = 3,
                AccCln61Week = 13,
                AccCln61Period = 3,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 3,
                AccWkend2N = 20240330,
                AccAltKeyNum = 1105
            },

            new CaldarRecord
            {
                AccWkendN = 240406,
                AccApWkend = 240413,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 20240406,
                AccAltKeyNum = 1106
            },

            new CaldarRecord
            {
                AccWkendN = 240413,
                AccApWkend = 240420,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 5,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 20240413,
                AccAltKeyNum = 1107
            },

            new CaldarRecord
            {
                AccWkendN = 240420,
                AccApWkend = 240427,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 20240420,
                AccAltKeyNum = 1108
            },

            new CaldarRecord
            {
                AccWkendN = 240427,
                AccApWkend = 240504,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 20240427,
                AccAltKeyNum = 1109
            },

            new CaldarRecord
            {
                AccWkendN = 240504,
                AccApWkend = 240511,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 20240504,
                AccAltKeyNum = 1110
            },

            new CaldarRecord
            {
                AccWkendN = 240511,
                AccApWkend = 240518,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 20240511,
                AccAltKeyNum = 1111
            },

            new CaldarRecord
            {
                AccWkendN = 240518,
                AccApWkend = 240525,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 20240518,
                AccAltKeyNum = 1112
            },

            new CaldarRecord
            {
                AccWkendN = 240525,
                AccApWkend = 240601,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 20240525,
                AccAltKeyNum = 1113
            },

            new CaldarRecord
            {
                AccWkendN = 240803,
                AccApWkend = 240810,
                AccWeekN = 31,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 31,
                AccCln60Period = 7,
                AccCln61Week = 31,
                AccCln61Period = 7,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 7,
                AccWkend2N = 20240803,
                AccAltKeyNum = 1123
            },

            new CaldarRecord
            {
                AccWkendN = 240810,
                AccApWkend = 240817,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 20240810,
                AccAltKeyNum = 1124
            },

            new CaldarRecord
            {
                AccWkendN = 240817,
                AccApWkend = 240824,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 20240817,
                AccAltKeyNum = 1125
            },

            new CaldarRecord
            {
                AccWkendN = 241005,
                AccApWkend = 241012,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 20241005,
                AccAltKeyNum = 1132
            },

            new CaldarRecord
            {
                AccWkendN = 241012,
                AccApWkend = 241019,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 20241012,
                AccAltKeyNum = 1133
            },

            new CaldarRecord
            {
                AccWkendN = 241019,
                AccApWkend = 241026,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 20241019,
                AccAltKeyNum = 1134
            },

            new CaldarRecord
            {
                AccWkendN = 241207,
                AccApWkend = 241214,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 20241207,
                AccAltKeyNum = 1141
            },

            new CaldarRecord
            {
                AccWkendN = 241214,
                AccApWkend = 241221,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 20241214,
                AccAltKeyNum = 1142
            },

            new CaldarRecord
            {
                AccWkendN = 241221,
                AccApWkend = 241228,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 20241221,
                AccAltKeyNum = 1143
            },

            new CaldarRecord
            {
                AccWkendN = 240106,
                AccApWkend = 240113,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 20240106,
                AccAltKeyNum = 1093
            },

            new CaldarRecord
            {
                AccWkendN = 240113,
                AccApWkend = 240120,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 20240113,
                AccAltKeyNum = 1094
            },

            new CaldarRecord
            {
                AccWkendN = 240120,
                AccApWkend = 240127,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 20240120,
                AccAltKeyNum = 1095
            },

            new CaldarRecord
            {
                AccWkendN = 240127,
                AccApWkend = 240203,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 20240127,
                AccAltKeyNum = 1096
            },

            new CaldarRecord
            {
                AccWkendN = 240203,
                AccApWkend = 240210,
                AccWeekN = 5,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 5,
                AccCln60Period = 1,
                AccCln61Week = 5,
                AccCln61Period = 1,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 1,
                AccWkend2N = 20240203,
                AccAltKeyNum = 1097
            },

            new CaldarRecord
            {
                AccWkendN = 240210,
                AccApWkend = 240217,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 20240210,
                AccAltKeyNum = 1098
            },

            new CaldarRecord
            {
                AccWkendN = 240601,
                AccApWkend = 240608,
                AccWeekN = 22,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 22,
                AccCln60Period = 5,
                AccCln61Week = 22,
                AccCln61Period = 5,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 5,
                AccWkend2N = 20240601,
                AccAltKeyNum = 1114
            },

            new CaldarRecord
            {
                AccWkendN = 240608,
                AccApWkend = 240615,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 20240608,
                AccAltKeyNum = 1115
            },

            new CaldarRecord
            {
                AccWkendN = 240615,
                AccApWkend = 240622,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 20240615,
                AccAltKeyNum = 1116
            },

            new CaldarRecord
            {
                AccWkendN = 240622,
                AccApWkend = 240629,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 20240622,
                AccAltKeyNum = 1117
            },

            new CaldarRecord
            {
                AccWkendN = 240706,
                AccApWkend = 240713,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 20240706,
                AccAltKeyNum = 1119
            },

            new CaldarRecord
            {
                AccWkendN = 240713,
                AccApWkend = 240720,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 3,
                AccCln61Week = 28,
                AccCln61Period = 3,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 3,
                AccWkend2N = 20240713,
                AccAltKeyNum = 1120
            },

            new CaldarRecord
            {
                AccWkendN = 240720,
                AccApWkend = 240727,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 3,
                AccCln61Week = 29,
                AccCln61Period = 3,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 3,
                AccWkend2N = 20240720,
                AccAltKeyNum = 1121
            },

            new CaldarRecord
            {
                AccWkendN = 240727,
                AccApWkend = 240803,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 20240727,
                AccAltKeyNum = 1122
            },

            new CaldarRecord
            {
                AccWkendN = 240824,
                AccApWkend = 240831,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 20240824,
                AccAltKeyNum = 1126
            },

            new CaldarRecord
            {
                AccWkendN = 240831,
                AccApWkend = 240907,
                AccWeekN = 35,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 35,
                AccCln60Period = 8,
                AccCln61Week = 35,
                AccCln61Period = 8,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 8,
                AccWkend2N = 20240831,
                AccAltKeyNum = 1127
            },

            new CaldarRecord
            {
                AccWkendN = 240907,
                AccApWkend = 240914,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 20240907,
                AccAltKeyNum = 1128
            },

            new CaldarRecord
            {
                AccWkendN = 240914,
                AccApWkend = 240921,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 20240914,
                AccAltKeyNum = 1129
            },

            new CaldarRecord
            {
                AccWkendN = 240921,
                AccApWkend = 240928,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 20240921,
                AccAltKeyNum = 1130
            },

            new CaldarRecord
            {
                AccWkendN = 240928,
                AccApWkend = 241005,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 20240928,
                AccAltKeyNum = 1131
            },

            new CaldarRecord
            {
                AccWkendN = 241026,
                AccApWkend = 241102,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 20241026,
                AccAltKeyNum = 1135
            },

            new CaldarRecord
            {
                AccWkendN = 241102,
                AccApWkend = 241109,
                AccWeekN = 44,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 44,
                AccCln60Period = 10,
                AccCln61Week = 44,
                AccCln61Period = 10,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 10,
                AccWkend2N = 20241102,
                AccAltKeyNum = 1136
            },

            new CaldarRecord
            {
                AccWkendN = 241109,
                AccApWkend = 241116,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 20241109,
                AccAltKeyNum = 1137
            },

            new CaldarRecord
            {
                AccWkendN = 241116,
                AccApWkend = 241123,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 20241116,
                AccAltKeyNum = 1138
            },

            new CaldarRecord
            {
                AccWkendN = 241123,
                AccApWkend = 241130,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 20241123,
                AccAltKeyNum = 1139
            },

            new CaldarRecord
            {
                AccWkendN = 241130,
                AccApWkend = 241207,
                AccWeekN = 48,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 48,
                AccCln60Period = 11,
                AccCln61Week = 48,
                AccCln61Period = 11,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 11,
                AccWkend2N = 20241130,
                AccAltKeyNum = 1140
            },

            new CaldarRecord
            {
                AccWkendN = 241228,
                AccApWkend = 250104,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20241228,
                AccAltKeyNum = 1144
            },

            new CaldarRecord
            {
                AccWkendN = 240629,
                AccApWkend = 240706,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 20240629,
                AccAltKeyNum = 1145
            },

            new CaldarRecord
            {
                AccWkendN = 990116,
                AccApWkend = 990123,
                AccWeekN = 2,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 2,
                AccCln60Period = 1,
                AccCln61Week = 2,
                AccCln61Period = 1,
                AccCln7XWeek = 2,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 2,
                AccCln6XPeriod = 1,
                AccWkend2N = 19990116,
                AccAltKeyNum = -406
            },

            new CaldarRecord
            {
                AccWkendN = 990123,
                AccApWkend = 990130,
                AccWeekN = 3,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 3,
                AccCln60Period = 1,
                AccCln61Week = 3,
                AccCln61Period = 1,
                AccCln7XWeek = 3,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 3,
                AccCln6XPeriod = 1,
                AccWkend2N = 19990123,
                AccAltKeyNum = -405
            },

            new CaldarRecord
            {
                AccWkendN = 990206,
                AccApWkend = 990213,
                AccWeekN = 5,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 5,
                AccCln60Period = 2,
                AccCln61Week = 5,
                AccCln61Period = 2,
                AccCln7XWeek = 5,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 5,
                AccCln6XPeriod = 2,
                AccWkend2N = 19990206,
                AccAltKeyNum = -403
            },

            new CaldarRecord
            {
                AccWkendN = 990213,
                AccApWkend = 990220,
                AccWeekN = 6,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 6,
                AccCln60Period = 2,
                AccCln61Week = 6,
                AccCln61Period = 2,
                AccCln7XWeek = 6,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 6,
                AccCln6XPeriod = 2,
                AccWkend2N = 19990213,
                AccAltKeyNum = -402
            },

            new CaldarRecord
            {
                AccWkendN = 990220,
                AccApWkend = 990227,
                AccWeekN = 7,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 7,
                AccCln60Period = 2,
                AccCln61Week = 7,
                AccCln61Period = 2,
                AccCln7XWeek = 7,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 7,
                AccCln6XPeriod = 2,
                AccWkend2N = 19990220,
                AccAltKeyNum = -401
            },

            new CaldarRecord
            {
                AccWkendN = 990306,
                AccApWkend = 990313,
                AccWeekN = 9,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 9,
                AccCln60Period = 3,
                AccCln61Week = 9,
                AccCln61Period = 3,
                AccCln7XWeek = 9,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 9,
                AccCln6XPeriod = 3,
                AccWkend2N = 19990306,
                AccAltKeyNum = -399
            },

            new CaldarRecord
            {
                AccWkendN = 990320,
                AccApWkend = 990327,
                AccWeekN = 11,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 11,
                AccCln60Period = 3,
                AccCln61Week = 11,
                AccCln61Period = 3,
                AccCln7XWeek = 11,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 11,
                AccCln6XPeriod = 3,
                AccWkend2N = 19990320,
                AccAltKeyNum = -397
            },

            new CaldarRecord
            {
                AccWkendN = 990410,
                AccApWkend = 990417,
                AccWeekN = 14,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 14,
                AccCln60Period = 4,
                AccCln61Week = 14,
                AccCln61Period = 4,
                AccCln7XWeek = 14,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 14,
                AccCln6XPeriod = 4,
                AccWkend2N = 19990410,
                AccAltKeyNum = -394
            },

            new CaldarRecord
            {
                AccWkendN = 990417,
                AccApWkend = 990424,
                AccWeekN = 15,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 15,
                AccCln60Period = 4,
                AccCln61Week = 15,
                AccCln61Period = 4,
                AccCln7XWeek = 15,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 15,
                AccCln6XPeriod = 4,
                AccWkend2N = 19990417,
                AccAltKeyNum = -393
            },

            new CaldarRecord
            {
                AccWkendN = 990424,
                AccApWkend = 990501,
                AccWeekN = 16,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 16,
                AccCln60Period = 4,
                AccCln61Week = 16,
                AccCln61Period = 4,
                AccCln7XWeek = 16,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 16,
                AccCln6XPeriod = 4,
                AccWkend2N = 19990424,
                AccAltKeyNum = -392
            },

            new CaldarRecord
            {
                AccWkendN = 990501,
                AccApWkend = 990508,
                AccWeekN = 17,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 17,
                AccCln60Period = 4,
                AccCln61Week = 17,
                AccCln61Period = 4,
                AccCln7XWeek = 17,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 17,
                AccCln6XPeriod = 4,
                AccWkend2N = 19990501,
                AccAltKeyNum = -391
            },

            new CaldarRecord
            {
                AccWkendN = 990522,
                AccApWkend = 990529,
                AccWeekN = 20,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 20,
                AccCln60Period = 5,
                AccCln61Week = 20,
                AccCln61Period = 5,
                AccCln7XWeek = 20,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 20,
                AccCln6XPeriod = 5,
                AccWkend2N = 19990522,
                AccAltKeyNum = -388
            },

            new CaldarRecord
            {
                AccWkendN = 990529,
                AccApWkend = 990605,
                AccWeekN = 21,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 21,
                AccCln60Period = 5,
                AccCln61Week = 21,
                AccCln61Period = 5,
                AccCln7XWeek = 21,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 21,
                AccCln6XPeriod = 5,
                AccWkend2N = 19990529,
                AccAltKeyNum = -387
            },

            new CaldarRecord
            {
                AccWkendN = 990612,
                AccApWkend = 990619,
                AccWeekN = 23,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 23,
                AccCln60Period = 6,
                AccCln61Week = 23,
                AccCln61Period = 6,
                AccCln7XWeek = 23,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 23,
                AccCln6XPeriod = 6,
                AccWkend2N = 19990612,
                AccAltKeyNum = -385
            },

            new CaldarRecord
            {
                AccWkendN = 990619,
                AccApWkend = 990626,
                AccWeekN = 24,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 24,
                AccCln60Period = 6,
                AccCln61Week = 24,
                AccCln61Period = 6,
                AccCln7XWeek = 24,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 24,
                AccCln6XPeriod = 6,
                AccWkend2N = 19990619,
                AccAltKeyNum = -384
            },

            new CaldarRecord
            {
                AccWkendN = 990626,
                AccApWkend = 990703,
                AccWeekN = 25,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 25,
                AccCln60Period = 6,
                AccCln61Week = 25,
                AccCln61Period = 6,
                AccCln7XWeek = 25,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 25,
                AccCln6XPeriod = 6,
                AccWkend2N = 19990626,
                AccAltKeyNum = -383
            },

            new CaldarRecord
            {
                AccWkendN = 990717,
                AccApWkend = 990724,
                AccWeekN = 28,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 28,
                AccCln60Period = 7,
                AccCln61Week = 28,
                AccCln61Period = 7,
                AccCln7XWeek = 28,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 28,
                AccCln6XPeriod = 7,
                AccWkend2N = 19990717,
                AccAltKeyNum = -380
            },

            new CaldarRecord
            {
                AccWkendN = 990724,
                AccApWkend = 990731,
                AccWeekN = 29,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 29,
                AccCln60Period = 7,
                AccCln61Week = 29,
                AccCln61Period = 7,
                AccCln7XWeek = 29,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 29,
                AccCln6XPeriod = 7,
                AccWkend2N = 19990724,
                AccAltKeyNum = -379
            },

            new CaldarRecord
            {
                AccWkendN = 990731,
                AccApWkend = 990807,
                AccWeekN = 30,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 30,
                AccCln60Period = 7,
                AccCln61Week = 30,
                AccCln61Period = 7,
                AccCln7XWeek = 30,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 30,
                AccCln6XPeriod = 7,
                AccWkend2N = 19990731,
                AccAltKeyNum = -378
            },

            new CaldarRecord
            {
                AccWkendN = 990814,
                AccApWkend = 990821,
                AccWeekN = 32,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 32,
                AccCln60Period = 8,
                AccCln61Week = 32,
                AccCln61Period = 8,
                AccCln7XWeek = 32,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 32,
                AccCln6XPeriod = 8,
                AccWkend2N = 19990814,
                AccAltKeyNum = -376
            },

            new CaldarRecord
            {
                AccWkendN = 990821,
                AccApWkend = 990828,
                AccWeekN = 33,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 33,
                AccCln60Period = 8,
                AccCln61Week = 33,
                AccCln61Period = 8,
                AccCln7XWeek = 33,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 33,
                AccCln6XPeriod = 8,
                AccWkend2N = 19990821,
                AccAltKeyNum = -375
            },

            new CaldarRecord
            {
                AccWkendN = 990904,
                AccApWkend = 990911,
                AccWeekN = 35,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 35,
                AccCln60Period = 9,
                AccCln61Week = 35,
                AccCln61Period = 9,
                AccCln7XWeek = 35,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 35,
                AccCln6XPeriod = 9,
                AccWkend2N = 19990904,
                AccAltKeyNum = -373
            },

            new CaldarRecord
            {
                AccWkendN = 990918,
                AccApWkend = 990925,
                AccWeekN = 37,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 37,
                AccCln60Period = 9,
                AccCln61Week = 37,
                AccCln61Period = 9,
                AccCln7XWeek = 37,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 37,
                AccCln6XPeriod = 9,
                AccWkend2N = 19990918,
                AccAltKeyNum = -371
            },

            new CaldarRecord
            {
                AccWkendN = 991009,
                AccApWkend = 991016,
                AccWeekN = 40,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 40,
                AccCln60Period = 10,
                AccCln61Week = 40,
                AccCln61Period = 10,
                AccCln7XWeek = 40,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 40,
                AccCln6XPeriod = 10,
                AccWkend2N = 19991009,
                AccAltKeyNum = -368
            },

            new CaldarRecord
            {
                AccWkendN = 991023,
                AccApWkend = 991030,
                AccWeekN = 42,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 42,
                AccCln60Period = 10,
                AccCln61Week = 42,
                AccCln61Period = 10,
                AccCln7XWeek = 42,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 42,
                AccCln6XPeriod = 10,
                AccWkend2N = 19991023,
                AccAltKeyNum = -366
            },

            new CaldarRecord
            {
                AccWkendN = 991030,
                AccApWkend = 991106,
                AccWeekN = 43,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 43,
                AccCln60Period = 10,
                AccCln61Week = 43,
                AccCln61Period = 10,
                AccCln7XWeek = 43,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 43,
                AccCln6XPeriod = 10,
                AccWkend2N = 19991030,
                AccAltKeyNum = -365
            },

            new CaldarRecord
            {
                AccWkendN = 991127,
                AccApWkend = 991204,
                AccWeekN = 47,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 47,
                AccCln60Period = 11,
                AccCln61Week = 47,
                AccCln61Period = 11,
                AccCln7XWeek = 47,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 47,
                AccCln6XPeriod = 11,
                AccWkend2N = 19991127,
                AccAltKeyNum = -361
            },

            new CaldarRecord
            {
                AccWkendN = 991204,
                AccApWkend = 991211,
                AccWeekN = 48,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 48,
                AccCln60Period = 12,
                AccCln61Week = 48,
                AccCln61Period = 12,
                AccCln7XWeek = 48,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 48,
                AccCln6XPeriod = 12,
                AccWkend2N = 19991204,
                AccAltKeyNum = -360
            },

            new CaldarRecord
            {
                AccWkendN = 991225,
                AccApWkend = 101,
                AccWeekN = 51,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 51,
                AccCln60Period = 12,
                AccCln61Week = 51,
                AccCln61Period = 12,
                AccCln7XWeek = 51,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 51,
                AccCln6XPeriod = 12,
                AccWkend2N = 19991225,
                AccAltKeyNum = -357
            },

            new CaldarRecord
            {
                AccWkendN = 101,
                AccApWkend = 108,
                AccWeekN = 52,
                AccPeriod = 12,
                AccQuarter = 1,
                AccCalPeriod = 12,
                AccCln60Week = 52,
                AccCln60Period = 12,
                AccCln61Week = 52,
                AccCln61Period = 12,
                AccCln7XWeek = 52,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 52,
                AccCln6XPeriod = 12,
                AccWkend2N = 20000101,
                AccAltKeyNum = -356
            },

            new CaldarRecord
            {
                AccWkendN = 990710,
                AccApWkend = 990717,
                AccWeekN = 27,
                AccPeriod = 7,
                AccQuarter = 3,
                AccCalPeriod = 7,
                AccCln60Week = 27,
                AccCln60Period = 7,
                AccCln61Week = 27,
                AccCln61Period = 7,
                AccCln7XWeek = 27,
                AccCln7XPeriod = 7,
                AccCln6XWeek = 27,
                AccCln6XPeriod = 7,
                AccWkend2N = 19990710,
                AccAltKeyNum = -381
            },

            new CaldarRecord
            {
                AccWkendN = 990807,
                AccApWkend = 990814,
                AccWeekN = 31,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 31,
                AccCln60Period = 8,
                AccCln61Week = 31,
                AccCln61Period = 8,
                AccCln7XWeek = 31,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 31,
                AccCln6XPeriod = 8,
                AccWkend2N = 19990807,
                AccAltKeyNum = -377
            },

            new CaldarRecord
            {
                AccWkendN = 990828,
                AccApWkend = 990904,
                AccWeekN = 34,
                AccPeriod = 8,
                AccQuarter = 3,
                AccCalPeriod = 8,
                AccCln60Week = 34,
                AccCln60Period = 8,
                AccCln61Week = 34,
                AccCln61Period = 8,
                AccCln7XWeek = 34,
                AccCln7XPeriod = 8,
                AccCln6XWeek = 34,
                AccCln6XPeriod = 8,
                AccWkend2N = 19990828,
                AccAltKeyNum = -374
            },

            new CaldarRecord
            {
                AccWkendN = 990911,
                AccApWkend = 990918,
                AccWeekN = 36,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 36,
                AccCln60Period = 9,
                AccCln61Week = 36,
                AccCln61Period = 9,
                AccCln7XWeek = 36,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 36,
                AccCln6XPeriod = 9,
                AccWkend2N = 19990911,
                AccAltKeyNum = -372
            },

            new CaldarRecord
            {
                AccWkendN = 990925,
                AccApWkend = 991002,
                AccWeekN = 38,
                AccPeriod = 9,
                AccQuarter = 3,
                AccCalPeriod = 9,
                AccCln60Week = 38,
                AccCln60Period = 9,
                AccCln61Week = 38,
                AccCln61Period = 9,
                AccCln7XWeek = 38,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 38,
                AccCln6XPeriod = 9,
                AccWkend2N = 19990925,
                AccAltKeyNum = -370
            },

            new CaldarRecord
            {
                AccWkendN = 991002,
                AccApWkend = 991009,
                AccWeekN = 39,
                AccPeriod = 9,
                AccQuarter = 4,
                AccCalPeriod = 9,
                AccCln60Week = 39,
                AccCln60Period = 9,
                AccCln61Week = 39,
                AccCln61Period = 9,
                AccCln7XWeek = 39,
                AccCln7XPeriod = 9,
                AccCln6XWeek = 39,
                AccCln6XPeriod = 9,
                AccWkend2N = 19991002,
                AccAltKeyNum = -369
            },

            new CaldarRecord
            {
                AccWkendN = 991016,
                AccApWkend = 991023,
                AccWeekN = 41,
                AccPeriod = 10,
                AccQuarter = 4,
                AccCalPeriod = 10,
                AccCln60Week = 41,
                AccCln60Period = 10,
                AccCln61Week = 41,
                AccCln61Period = 10,
                AccCln7XWeek = 41,
                AccCln7XPeriod = 10,
                AccCln6XWeek = 41,
                AccCln6XPeriod = 10,
                AccWkend2N = 19991016,
                AccAltKeyNum = -367
            },

            new CaldarRecord
            {
                AccWkendN = 991106,
                AccApWkend = 991113,
                AccWeekN = 44,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 44,
                AccCln60Period = 11,
                AccCln61Week = 44,
                AccCln61Period = 11,
                AccCln7XWeek = 44,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 44,
                AccCln6XPeriod = 11,
                AccWkend2N = 19991106,
                AccAltKeyNum = -364
            },

            new CaldarRecord
            {
                AccWkendN = 991113,
                AccApWkend = 991120,
                AccWeekN = 45,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 45,
                AccCln60Period = 11,
                AccCln61Week = 45,
                AccCln61Period = 11,
                AccCln7XWeek = 45,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 45,
                AccCln6XPeriod = 11,
                AccWkend2N = 19991113,
                AccAltKeyNum = -363
            },

            new CaldarRecord
            {
                AccWkendN = 991120,
                AccApWkend = 991127,
                AccWeekN = 46,
                AccPeriod = 11,
                AccQuarter = 4,
                AccCalPeriod = 11,
                AccCln60Week = 46,
                AccCln60Period = 11,
                AccCln61Week = 46,
                AccCln61Period = 11,
                AccCln7XWeek = 46,
                AccCln7XPeriod = 11,
                AccCln6XWeek = 46,
                AccCln6XPeriod = 11,
                AccWkend2N = 19991120,
                AccAltKeyNum = -362
            },

            new CaldarRecord
            {
                AccWkendN = 991211,
                AccApWkend = 991218,
                AccWeekN = 49,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 49,
                AccCln60Period = 12,
                AccCln61Week = 49,
                AccCln61Period = 12,
                AccCln7XWeek = 49,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 49,
                AccCln6XPeriod = 12,
                AccWkend2N = 19991211,
                AccAltKeyNum = -359
            },

            new CaldarRecord
            {
                AccWkendN = 991218,
                AccApWkend = 991225,
                AccWeekN = 50,
                AccPeriod = 12,
                AccQuarter = 4,
                AccCalPeriod = 12,
                AccCln60Week = 50,
                AccCln60Period = 12,
                AccCln61Week = 50,
                AccCln61Period = 12,
                AccCln7XWeek = 50,
                AccCln7XPeriod = 12,
                AccCln6XWeek = 50,
                AccCln6XPeriod = 12,
                AccWkend2N = 19991218,
                AccAltKeyNum = -358
            },

            new CaldarRecord
            {
                AccWkendN = 990109,
                AccApWkend = 990116,
                AccWeekN = 1,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 1,
                AccCln60Period = 1,
                AccCln61Week = 1,
                AccCln61Period = 1,
                AccCln7XWeek = 1,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 1,
                AccCln6XPeriod = 1,
                AccWkend2N = 19990109,
                AccAltKeyNum = -407
            },

            new CaldarRecord
            {
                AccWkendN = 990130,
                AccApWkend = 990206,
                AccWeekN = 4,
                AccPeriod = 1,
                AccQuarter = 1,
                AccCalPeriod = 1,
                AccCln60Week = 4,
                AccCln60Period = 1,
                AccCln61Week = 4,
                AccCln61Period = 1,
                AccCln7XWeek = 4,
                AccCln7XPeriod = 1,
                AccCln6XWeek = 4,
                AccCln6XPeriod = 1,
                AccWkend2N = 19990130,
                AccAltKeyNum = -404
            },

            new CaldarRecord
            {
                AccWkendN = 990227,
                AccApWkend = 990306,
                AccWeekN = 8,
                AccPeriod = 2,
                AccQuarter = 1,
                AccCalPeriod = 2,
                AccCln60Week = 8,
                AccCln60Period = 2,
                AccCln61Week = 8,
                AccCln61Period = 2,
                AccCln7XWeek = 8,
                AccCln7XPeriod = 2,
                AccCln6XWeek = 8,
                AccCln6XPeriod = 2,
                AccWkend2N = 19990227,
                AccAltKeyNum = -400
            },

            new CaldarRecord
            {
                AccWkendN = 990313,
                AccApWkend = 990320,
                AccWeekN = 10,
                AccPeriod = 3,
                AccQuarter = 1,
                AccCalPeriod = 3,
                AccCln60Week = 10,
                AccCln60Period = 3,
                AccCln61Week = 10,
                AccCln61Period = 3,
                AccCln7XWeek = 10,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 10,
                AccCln6XPeriod = 3,
                AccWkend2N = 19990313,
                AccAltKeyNum = -398
            },

            new CaldarRecord
            {
                AccWkendN = 990327,
                AccApWkend = 990403,
                AccWeekN = 12,
                AccPeriod = 3,
                AccQuarter = 2,
                AccCalPeriod = 3,
                AccCln60Week = 12,
                AccCln60Period = 3,
                AccCln61Week = 12,
                AccCln61Period = 3,
                AccCln7XWeek = 12,
                AccCln7XPeriod = 3,
                AccCln6XWeek = 12,
                AccCln6XPeriod = 3,
                AccWkend2N = 19990327,
                AccAltKeyNum = -396
            },

            new CaldarRecord
            {
                AccWkendN = 990403,
                AccApWkend = 990410,
                AccWeekN = 13,
                AccPeriod = 4,
                AccQuarter = 2,
                AccCalPeriod = 4,
                AccCln60Week = 13,
                AccCln60Period = 4,
                AccCln61Week = 13,
                AccCln61Period = 4,
                AccCln7XWeek = 13,
                AccCln7XPeriod = 4,
                AccCln6XWeek = 13,
                AccCln6XPeriod = 4,
                AccWkend2N = 19990403,
                AccAltKeyNum = -395
            },

            new CaldarRecord
            {
                AccWkendN = 990508,
                AccApWkend = 990515,
                AccWeekN = 18,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 18,
                AccCln60Period = 5,
                AccCln61Week = 18,
                AccCln61Period = 5,
                AccCln7XWeek = 18,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 18,
                AccCln6XPeriod = 5,
                AccWkend2N = 19990508,
                AccAltKeyNum = -390
            },

            new CaldarRecord
            {
                AccWkendN = 990515,
                AccApWkend = 990522,
                AccWeekN = 19,
                AccPeriod = 5,
                AccQuarter = 2,
                AccCalPeriod = 5,
                AccCln60Week = 19,
                AccCln60Period = 5,
                AccCln61Week = 19,
                AccCln61Period = 5,
                AccCln7XWeek = 19,
                AccCln7XPeriod = 5,
                AccCln6XWeek = 19,
                AccCln6XPeriod = 5,
                AccWkend2N = 19990515,
                AccAltKeyNum = -389
            },

            new CaldarRecord
            {
                AccWkendN = 990605,
                AccApWkend = 990612,
                AccWeekN = 22,
                AccPeriod = 6,
                AccQuarter = 2,
                AccCalPeriod = 6,
                AccCln60Week = 22,
                AccCln60Period = 6,
                AccCln61Week = 22,
                AccCln61Period = 6,
                AccCln7XWeek = 22,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 22,
                AccCln6XPeriod = 6,
                AccWkend2N = 19990605,
                AccAltKeyNum = -386
            },

            new CaldarRecord
            {
                AccWkendN = 990703,
                AccApWkend = 990710,
                AccWeekN = 26,
                AccPeriod = 6,
                AccQuarter = 3,
                AccCalPeriod = 6,
                AccCln60Week = 26,
                AccCln60Period = 6,
                AccCln61Week = 26,
                AccCln61Period = 6,
                AccCln7XWeek = 26,
                AccCln7XPeriod = 6,
                AccCln6XWeek = 26,
                AccCln6XPeriod = 6,
                AccWkend2N = 19990703,
                AccAltKeyNum = -382
            }

        );
    }
}

  

