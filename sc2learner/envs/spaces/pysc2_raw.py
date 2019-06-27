from __future__ import absolute_import
from __future__ import division
from __future__ import print_function

import gym

## 用于描述有效的运动和观测的格式和范围
class PySC2RawAction(gym.Space):
  pass


##
class PySC2RawObservation(gym.Space):

  # sc2_env.observation_spec
  def __init__(self, observation_spec_fn):
    self._feature_layers = observation_spec_fn()

  @property
  def space_attr(self):
    return self._feature_layers
