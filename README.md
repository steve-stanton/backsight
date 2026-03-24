# Overview

Backsight is a data entry mechanism for producing cadastral map data from hardcopy survey plans, and was originally developed in the
late 1990s for the Canadian province of Manitoba. Some of these plans date back to the 19th century, and show thedistances and angles
that were used to subdivide the land.

<img src="docs/images/plan-detail.jpg" width="500" />

# Basic Requirements

## Data Entry

Backsight should make it easy to convert old survey plans into digital maps, while also preserving the original observation details
found on the hardcopy plans. For example, a survey plan may indicate that a point was positioned having turned a 90 degree angle with
respect to a reference point. Or you may have a line that has been laid out exactly parallel to another line.

## Updates

The original observations (distances and angles) should act as constraints that are taken into account during
any sort of updates.

In a situation where the ground control has been adjusted (e.g. see [this](https://www.ngs.noaa.gov/datums/newdatums/what-to-expect.shtml)), it should be possible to recalculate the geometry for spatial objects while taking these constraints into account. When Backsight does this, it will be allowed to scale the dimensions of lines, but must continue to honour observed angles and offset distances.

Comparing the adjusted length of each line with the originally observed length will provide a way to confirm that the update continues to achieve an acceptable precision.

## Derived Layers

While a parcel appearing on a survey plan will frequently have a specific owner, this is not always the case.
Someone may well sell their property (or some portion of it) to their neighbour, leading to property boundaries
that no longer coincide with the original plans. This can complicate matters if land survey and property ownership
are handled by different government agencies. Some amount of coordination is needed to ensure that changes are
correctly conveyed from one department to another.

A further complication arises because different portions of a single property may be assessed differently for
the purpose of taxation. For example, a large supermarket car park may contain a forecourt area that is
subject to a different tax rate.

A hierarchy of map layers will be used to deal with this, as shown below.

<img src="docs/images/layers.png" width="300" />

It must be possible to propagate changes made on the survey layer to the ownership layer, and then
to the tax assessment layer. Changes going the other way must not be permitted.
