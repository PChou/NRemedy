#ifndef ARFILTERAPI_H
#define ARFILTERAPI_H

#include "ar.h"
#include "arplugin.h"

#define ARFILTERAPI_PLUGIN_VERSION        1

EXTERN_FUNCTION (ARPLUGIN_EXPORT void ARFilterApiCall, (
                              void               *object,
                              ARValueList        *inValues,
                              ARValueList        *outValues,
                              ARStatusList       *status));

#endif /* ARFILTERAPI_H */
