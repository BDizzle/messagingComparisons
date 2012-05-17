namespace csharp TRMS.MessagingComparisons.Thrift

struct Command {
1: string commandName,
2: i32 region,
3: string videoFileName,
4: string audioFileName0,
5: string audioFileName1,
6: string audioFileName2,
7: string audioFileName3,
8: string vbiFileName,
9: bool useTDIR,
10: i64 initialFrame,
11: double initialRate,
12: bool loop
}