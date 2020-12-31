# Bill Death Index

A tool for identifying legislative bills that have died (either in committee or through some other means). The tool is originally built to work with the New York State Open Legislation API, but is capable of expanding to include additional APIs.

## Death Level Classifications

Bills are classified to have four different levels of death. Each level requires certain conditions to be met, with the most stringent conditions being checked first. (i.e. if a bill meets the conditions for more than one death level classification, it'll take on the first one in the order listed below)

| Level                   | Conditions                                                                                                                                                                                                                        |
|-------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Six Feet Under (Severe) | - Last referred to a committee.<br>- Inactive for the past year.<br>- One of the following:<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- Majority of cosponsors are out of office and the bill's been inactive for six months.<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- All cosponsors are out of office. |
| Clinical (High)         | - One of the following:<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- Pocket vetoed by the governor.<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- Vetoed by the governor and inactive for a year.                                                                                                              |
| Flatlining (Moderate)   | - Referred to a committee.<br>- Inactive for a year.                                                                                                                                                                              |
| Mostly Dead (Low)       | - Inactive for a year.<br>- One of the following:<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- Approved by a committee.<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;- Approved by a chamber.                                                                                                                   |

## Viewing the Results

The results can be viewed on a Github pages website that allows for searching, sorting and filtering the various bills considered to be dead. [Coming Soon]()
