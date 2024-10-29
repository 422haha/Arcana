package com.arcane.arcana.subscribe.service;

import com.arcane.arcana.common.entity.Subscribe;
import com.arcane.arcana.subscribe.repository.SubscribeRepository;
import com.arcane.arcana.common.exception.CustomException;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
public class SubscribeServiceImpl implements SubscribeService {

    private final SubscribeRepository subscribeRepository;

    @Autowired
    public SubscribeServiceImpl(SubscribeRepository subscribeRepository) {
        this.subscribeRepository = subscribeRepository;
    }

    @Override
    @Transactional
    public void subscribe(String email) {
        if (subscribeRepository.existsBySubscribe(email)) {
            throw new CustomException("이미 구독된 이메일입니다.", HttpStatus.BAD_REQUEST);
        }
        Subscribe subscribe = new Subscribe();
        subscribe.setSubscribe(email);
        subscribeRepository.save(subscribe);
    }
}
