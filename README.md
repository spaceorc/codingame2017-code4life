# codingame2017-code4life
Solution for https://www.codingame.com/contests/code4life/

## Game
It's a solution itself.

- *Types* - all supported entities
- *State* - turn state and game state
- *Strategy* - most important part of solution - strategy for acquiring samples and gathering molecules.

## Implemented strategy

Very simple. A lot of heuristics. No simulation (tried but was considered bad...)

*GatherOrder* evaluates each gathering order, provided by *IVariantSource*, and returns a lot
of information for each variant, such as turns required to complete this order and scorepoints got from it.

There are two main strategies working atop of this available gathering orders:
- *AcquireStrategy* chooses the best variant of collecting new samples, either from SAMPLES module, or from DIAGNOSIS module.
- *GatherStrategy* starts when robot has already acquired chosen samples and ready to gather molecules or produce that samples.

One more strategy, called *ProduceStrategy*, starts when there is no more molecules to gather, and no more molecules
will come from enemy being produced his samples. This strategy moves robot to LABORATORY module and produces collected 
samples in s best way, defined in *ProduceOrder* and *ProduceOrderDefaultComparer* classes.

## Pack
Packs all .cs files in solution into one .cs file as it required by codingame.

## Experiments
Project for experiments. Entry point for replaying game turns from dumps
